
export function setTheme(darkMode, hue) {
    const saturation = 60;

    const body = document.getElementsByTagName('body')[0];
    const element = document.getElementById('app');

    body.style.setProperty('--color-white', darkMode ? getHsl(0, 0, 10) : getHsl(0, 0, 100));
    body.style.setProperty('--color-black', darkMode ? getHsl(hue, saturation, 97) : getHsl(hue, saturation, 20));

    element.style.setProperty('--color-black', darkMode ? getHsl(hue, saturation, 97) : getHsl(hue, saturation, 20));
    element.style.setProperty('--color-dark', darkMode ? getHsl(hue, saturation, 80) : getHsl(hue, saturation, 30));
    element.style.setProperty('--color-secondary', darkMode ? getHsl(hue, saturation, 40) : getHsl(hue, saturation, 80));
    element.style.setProperty('--color-light', darkMode ? getHsl(hue, saturation, 30) : getHsl(hue, saturation, 97));
    element.style.setProperty('--color-white', darkMode ? getHsl(0, 0, 10) : getHsl(0, 0, 100));

    element.style.setProperty('--color-shadow', darkMode ? getHsla(hue, saturation, 75, 0.3) : getHsla(hue, saturation, 0, 0.3));
    element.style.setProperty('--color-shadow-light', darkMode ? getHsla(hue, saturation, 75, 0.3) : getHsla(hue, saturation, 0, 0.15));

    element.style.setProperty('--color-good', darkMode ? getHsl(128, 50, 50) : getHsl(128, 50, 80));
    element.style.setProperty('--color-good-light', darkMode ? getHsl(128, 50, 30) : getHsl(128, 50, 90));
    element.style.setProperty('--color-bad', darkMode ? getHsl(0, 50, 50) : getHsl(0, 50, 80));
    element.style.setProperty('--color-bad-light', darkMode ? getHsl(0, 50, 30) : getHsl(0, 50, 90));
    element.style.setProperty('--color-bad-dark', darkMode ? getHsl(0, 50, 60) : getHsl(0, 50, 50));
}

export function preferesDarkMode() {
    const matched = window.matchMedia('(prefers-color-scheme: dark)').matches;
    return matched;
}

function getHsl(h, s, l) {
    return `hsl(${h}, ${s}%, ${l}%)`;
}

function getHsla(h, s, l, a) {
    return `hsl(${h}, ${s}%, ${l}%, ${a})`;
}
