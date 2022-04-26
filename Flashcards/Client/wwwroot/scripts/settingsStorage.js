
export function setLocalValue(key, value) {
    localStorage.setItem(key, value);
}

export function getLocalValue(key) {
    return localStorage.getItem(key);
}
