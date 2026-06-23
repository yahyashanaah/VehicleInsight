# AWS EC2 Deployment Guide

This guide explains how to deploy VehicleInsight to an Ubuntu EC2 free tier instance using Docker Compose.

## Target Setup

- AWS EC2 instance: Ubuntu Server, free tier eligible
- Instance type: `t2.micro` or `t3.micro`, depending on free tier availability in your AWS account/region
- Public app port: `80`
- Container app port: `8080`
- Runtime: Docker Compose
- Current live URL: `http://13.51.175.53`
- App URL format: `http://<ec2-public-ip>`

The application container listens on port `8080`. The current AWS deployment exposes it publicly on standard HTTP port `80`, so users can open the app without adding `:8080` to the URL.

## 1. Create An EC2 Instance

1. Sign in to the AWS Console.
2. Open the EC2 service.
3. Choose `Launch instance`.
4. Enter a name, for example `vehicleinsight-web`.
5. Select an Ubuntu Server AMI marked as free tier eligible.
6. Select a free tier eligible instance type, such as `t2.micro` or `t3.micro`.
7. Create or select an existing key pair.
8. Download the `.pem` private key if creating a new key pair.
9. Configure the security group rules as described below.
10. Launch the instance.

## 2. Configure The Security Group

The EC2 security group must allow SSH for administration and HTTP port `80` for the public app.

Recommended inbound rules:

| Type | Protocol | Port | Source | Purpose |
| --- | --- | --- | --- | --- |
| SSH | TCP | 22 | Your IP address | Connect to the instance |
| HTTP | TCP | 80 | `0.0.0.0/0` | Access VehicleInsight |

Notes:

- For SSH, prefer `My IP` instead of opening port `22` to the world.
- The live app is publicly available at `http://13.51.175.53` on port `80`.
- The container still listens on port `8080`; the EC2 host maps public port `80` to container port `8080`.
- Outbound traffic can remain at the default setting so the instance can pull Docker packages and call the NHTSA VPIC API.

## 3. Connect To The Instance

From your local machine, connect with SSH.

On macOS/Linux/Git Bash:

```bash
chmod 400 /path/to/key.pem
ssh -i /path/to/key.pem ubuntu@<ec2-public-ip>
```

On Windows PowerShell:

```powershell
ssh -i C:\path\to\key.pem ubuntu@<ec2-public-ip>
```

Replace `<ec2-public-ip>` with the public IPv4 address shown in the EC2 console.

## 4. Install Docker

Run these commands on the EC2 instance:

```bash
sudo apt-get update
sudo apt-get install -y ca-certificates curl
sudo install -m 0755 -d /etc/apt/keyrings
sudo curl -fsSL https://download.docker.com/linux/ubuntu/gpg -o /etc/apt/keyrings/docker.asc
sudo chmod a+r /etc/apt/keyrings/docker.asc

echo \
  "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.asc] https://download.docker.com/linux/ubuntu \
  $(. /etc/os-release && echo "${UBUNTU_CODENAME:-$VERSION_CODENAME}") stable" | \
  sudo tee /etc/apt/sources.list.d/docker.list > /dev/null

sudo apt-get update
sudo apt-get install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
```

Verify Docker:

```bash
sudo docker --version
sudo docker compose version
```

Optional: allow the `ubuntu` user to run Docker without `sudo`.

```bash
sudo usermod -aG docker ubuntu
exit
```

Reconnect with SSH after running the command above, then verify:

```bash
docker ps
```

## 5. Clone The GitHub Repository

Install Git if needed:

```bash
sudo apt-get install -y git
```

Clone the repository:

```bash
git clone https://github.com/<your-github-user>/<your-repo-name>.git
cd <your-repo-name>
```

Replace the GitHub URL with the actual VehicleInsight repository URL.

## 6. Run The App With Docker Compose

From the solution root on the EC2 instance:

```bash
docker compose up --build -d
```

If you did not add the `ubuntu` user to the Docker group, use:

```bash
sudo docker compose up --build -d
```

Check container status:

```bash
docker compose ps
```

For the current live AWS deployment, the app is exposed publicly on port `80` and forwarded to container port `8080`. The container port remains `8080`; the public URL is standard HTTP.

The EC2 deployment should show a port mapping similar to:

```text
0.0.0.0:80->8080/tcp
```

If your EC2 instance shows `0.0.0.0:8080->8080/tcp`, the app will be available at `http://<ec2-public-ip>:8080` instead. Update the EC2 deployment port mapping to `80:8080` to match the current live URL.

## 7. Verify The App Is Running

From the EC2 instance:

```bash
curl http://localhost:8080/health
```

Expected response:

```text
Healthy
```

From your browser:

- UI: `http://<ec2-public-ip>`
- Current live UI: `http://13.51.175.53`

Swagger is intended for local/Docker development URLs. The current public EC2 URL serves the UI and API but does not expose Swagger publicly.

If the browser cannot connect:

1. Confirm the container is running with `docker compose ps`.
2. Confirm the security group allows inbound HTTP TCP `80`.
3. Confirm you are using the instance public IPv4 address.
4. Confirm the host maps public port `80` to container port `8080`.
5. Check logs with the commands below.

## 8. View Logs

Show recent logs:

```bash
docker compose logs --tail=100 vehicleinsight.web
```

Follow live logs:

```bash
docker compose logs -f vehicleinsight.web
```

## 9. Stop And Restart The App

Stop the app:

```bash
docker compose down
```

Start it again:

```bash
docker compose up -d
```

Rebuild after pulling new code:

```bash
git pull
docker compose up --build -d
```

Restart without rebuilding:

```bash
docker compose restart vehicleinsight.web
```

## 10. Useful Maintenance Commands

Check running containers:

```bash
docker ps
```

Check disk usage:

```bash
docker system df
```

Remove unused Docker build cache/images:

```bash
docker system prune
```

Use prune commands carefully on shared servers because they remove unused Docker resources.

## References

- AWS EC2 security groups: https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/ec2-security-groups.html
- AWS SSH connection guide: https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/connect-linux-inst-ssh.html
- Docker Engine on Ubuntu: https://docs.docker.com/engine/install/ubuntu/
