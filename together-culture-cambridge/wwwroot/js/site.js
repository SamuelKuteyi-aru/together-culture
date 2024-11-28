// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
const isValidEmail = (email) => {
    return emailRegex.test(email)
}

const getErrorTextElement = (field) => {
    let section = field.closest(".input-section");
    if (section && section.querySelector(".error-text")) {
        return section.querySelector(".error-text");
    }

    return null

}

const generateHeaders = () => {
    let verificationTokenField = $("input[name='__RequestVerificationToken']");
    let token;
    if (verificationTokenField.length > 0) {
        token = verificationTokenField[0].value;
    }
    const headers = new Headers();
    headers.append("Accept", "application/json");
    headers.append("Content-Type", "application/json");

    if (token) {
        headers.append("RequestVerificationToken", token);
    }

    return headers;
}

const debounce = (fn, wait, immediate = false) => {
    let timer;
    return function (...args) {
        const callNow = immediate && !timer;
        clearTimeout(timer);
        timer = setTimeout(() => {
            timer = null;
            if (!immediate) fn.apply(this, args);
        }, wait);
        if (callNow) fn.apply(this, args);
    };
};