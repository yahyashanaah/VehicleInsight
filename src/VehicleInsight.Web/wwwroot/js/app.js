const makeSelect = document.querySelector("#make-select");
const yearInput = document.querySelector("#year-input");
const form = document.querySelector("#vehicle-search-form");
const searchButton = document.querySelector("#search-button");
const makesStatus = document.querySelector("#makes-status");
const message = document.querySelector("#message");
const vehicleTypes = document.querySelector("#vehicle-types");
const vehicleModels = document.querySelector("#vehicle-models");
const typesCount = document.querySelector("#types-count");
const modelsCount = document.querySelector("#models-count");

const minimumYear = 1900;
const maximumYear = new Date().getFullYear() + 1;

yearInput.max = String(maximumYear);

document.addEventListener("DOMContentLoaded", loadMakes);
form.addEventListener("submit", handleSearch);

async function loadMakes() {
    setMakesLoading(true);
    clearMessage();

    try {
        const makes = await fetchJson("/api/vehicles/makes");
        populateMakes(makes);
        makesStatus.textContent = `${makes.length} makes loaded`;
        makesStatus.classList.add("ready");
    } catch (error) {
        makeSelect.innerHTML = '<option value="">Unable to load makes</option>';
        showError(error.message);
        makesStatus.textContent = "Makes unavailable";
    } finally {
        setMakesLoading(false);
    }
}

async function handleSearch(event) {
    event.preventDefault();

    const validationError = validateSearch();

    if (validationError) {
        showError(validationError);
        return;
    }

    const makeId = makeSelect.value;
    const year = yearInput.value.trim();

    setSearching(true);
    showInfo("Searching vehicle data...");
    clearResults();

    try {
        const [types, models] = await Promise.all([
            fetchJson(`/api/vehicles/makes/${makeId}/types`),
            fetchJson(`/api/vehicles/makes/${makeId}/models?year=${encodeURIComponent(year)}`)
        ]);

        renderVehicleTypes(types);
        renderVehicleModels(models);
        clearMessage();
    } catch (error) {
        showError(error.message);
    } finally {
        setSearching(false);
    }
}

function populateMakes(makes) {
    const options = ['<option value="">Select a make</option>'];

    for (const make of makes) {
        const makeId = make.makeId ?? make.MakeId;
        const makeName = make.makeName ?? make.MakeName;

        if (!makeId || !makeName) {
            continue;
        }

        options.push(`<option value="${escapeHtml(String(makeId))}">${escapeHtml(makeName)}</option>`);
    }

    makeSelect.innerHTML = options.join("");
}

function validateSearch() {
    const makeId = Number(makeSelect.value);
    const year = Number(yearInput.value);

    if (!makeSelect.value || !Number.isInteger(makeId) || makeId <= 0) {
        return "Select a vehicle make before searching.";
    }

    if (!Number.isInteger(year) || year < minimumYear || year > maximumYear) {
        return `Enter a manufacture year between ${minimumYear} and ${maximumYear}.`;
    }

    return "";
}

function renderVehicleTypes(types) {
    typesCount.textContent = String(types.length);

    if (!types.length) {
        vehicleTypes.className = "result-list empty-state";
        vehicleTypes.textContent = "No vehicle types found for this make.";
        return;
    }

    vehicleTypes.className = "result-list";
    vehicleTypes.innerHTML = types.map((type) => {
        const name = type.vehicleTypeName ?? type.VehicleTypeName ?? "Unknown type";
        const id = type.vehicleTypeId ?? type.VehicleTypeId;

        return `
            <article class="result-item">
                <p class="result-title">${escapeHtml(name)}</p>
                <p class="result-meta">Type ID: ${escapeHtml(String(id ?? "N/A"))}</p>
            </article>
        `;
    }).join("");
}

function renderVehicleModels(models) {
    modelsCount.textContent = String(models.length);

    if (!models.length) {
        vehicleModels.className = "result-list empty-state";
        vehicleModels.textContent = "No models found for this make and year.";
        return;
    }

    vehicleModels.className = "result-list";
    vehicleModels.innerHTML = models.map((model) => {
        const modelName = model.modelName ?? model.ModelName ?? "Unknown model";
        const modelId = model.modelId ?? model.ModelId;
        const makeName = model.makeName ?? model.MakeName;

        return `
            <article class="result-item">
                <p class="result-title">${escapeHtml(modelName)}</p>
                <p class="result-meta">${escapeHtml(makeName ?? "Selected make")} · Model ID: ${escapeHtml(String(modelId ?? "N/A"))}</p>
            </article>
        `;
    }).join("");
}

async function fetchJson(url) {
    let response;

    try {
        response = await fetch(url, {
            headers: {
                Accept: "application/json"
            }
        });
    } catch {
        throw new Error("The API could not be reached. Check that the server is running and try again.");
    }

    if (!response.ok) {
        const details = await readErrorResponse(response);
        throw new Error(details || `The API returned ${response.status}.`);
    }

    return response.json();
}

async function readErrorResponse(response) {
    const contentType = response.headers.get("content-type") ?? "";

    if (contentType.includes("application/json")) {
        const body = await response.json();
        return body.title ?? body.detail ?? JSON.stringify(body);
    }

    return response.text();
}

function setMakesLoading(isLoading) {
    makeSelect.disabled = isLoading;
    searchButton.disabled = isLoading;

    if (isLoading) {
        makesStatus.textContent = "Loading makes...";
        makeSelect.innerHTML = '<option value="">Loading vehicle makes...</option>';
    }
}

function setSearching(isSearching) {
    makeSelect.disabled = isSearching;
    yearInput.disabled = isSearching;
    searchButton.disabled = isSearching;
    searchButton.textContent = isSearching ? "Searching..." : "Search";
}

function clearResults() {
    typesCount.textContent = "0";
    modelsCount.textContent = "0";
    vehicleTypes.className = "result-list empty-state";
    vehicleTypes.textContent = "Loading vehicle types...";
    vehicleModels.className = "result-list empty-state";
    vehicleModels.textContent = "Loading models...";
}

function showError(text) {
    message.hidden = false;
    message.className = "message error";
    message.textContent = text;
}

function showInfo(text) {
    message.hidden = false;
    message.className = "message info";
    message.textContent = text;
}

function clearMessage() {
    message.hidden = true;
    message.textContent = "";
    message.className = "message";
}

function escapeHtml(value) {
    return value.replace(/[&<>"']/g, (character) => ({
        "&": "&amp;",
        "<": "&lt;",
        ">": "&gt;",
        '"': "&quot;",
        "'": "&#039;"
    })[character]);
}
