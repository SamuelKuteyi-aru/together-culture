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

const getBreadcrumbPage = () => {
    let href = window.location.href;
    let hrefSplit = href.split('/');
    return hrefSplit[hrefSplit.length - 1]
}

const cancelSpaceBooking = async (spaceId) => {
    let response = await fetch("/SpaceBooking/Cancel", {
        method: "POST",
        headers: {
            Accept: 'application/json',
            'Content-Type': 'application/json',
        },
        credentials: 'include',
        body: JSON.stringify({ spaceId })
    })

    let body = await response.json();

    if (!response.ok) {
        throw new Error(body.message);
    }

    return body;
}
const bookSlotAtSpace = async (spaceId, bookingDate) => {
    let response = await fetch("/SpaceBooking/Create", {
        method: "POST",
        headers: {
            Accept: 'application/json',
            'Content-Type': 'application/json',
        },
        credentials: 'include',
        body: JSON.stringify({
            spaceId,
            bookingDate
        })
    })

    let body = await response.json();
    if (!response.ok) {
        throw new Error(body.message);
    }

    return body;
}


const bookEventSlot = async (eventId) => {
    let response = await fetch("/EventBooking/Create", {
        method: "POST",
        headers: {
            Accept: 'application/json',
            'Content-Type': 'application/json',
        },
        credentials: 'include',
        body: JSON.stringify({ eventId })
    })

    let body = await response.json();
    if (!response.ok) {
        throw new Error(body.message);
    }

    return body;
}

const cancelEventBooking = async (eventId) => {
    let response = await fetch("/EventBooking/Cancel", {
        method: "POST",
        headers: {
            Accept: 'application/json',
            'Content-Type': 'application/json',
        },
        credentials: 'include',
        body: JSON.stringify({ eventId })
    })

    let body = await response.json();
    if (!response.ok) {
        throw new Error(body.message);
    }

    return body;
}

const createTimeItemString = (value) => {
    return value.toString().length === 1 ? `0${value}` : value.toString()
}
const getTimeString = (time) => {
    let dateTime = new Date(time)
    return createTimeItemString(dateTime.getHours()) + ":" + createTimeItemString(dateTime.getMinutes());
}
const createSpaceDataList = (gridSection, list, userType) => {
    gridSection.html("");
    
    console.log({ gridSection, list })
    list.forEach(space => {
        let closingTime = getTimeString(space.closingTime);
        let openingTime = getTimeString(space.openingTime);

        gridSection.append(`
            <div class="grid-item">
                <div class="img-icon">
                    <img alt="user" src="/icons/shuttle.svg"/>
                </div>
                <div class="name my-2">${ space.roomId }</div>
                <div class="date opacity-75 mt-1">
                    Opens at ${ openingTime }, Closes at ${ closingTime }
                </div>
                
                 <div class="data-row my-2">
                    ${ space.totalSeats - space.bookedSeats } seats open today
                </div>
                
                ${
            userType === 'Admin' ?
                `<button class="button button-secondary space-edit-btn">
                                Edit Space
                        </button>
                         
                         
                        <button class="delete-icon">
                            <box-icon class="icon" name="trash"></box-icon>
                        </button>
                    ` : userType === 'EndUser' ? `
                        <button class="button button-secondary space-view-btn mt-2">
                            View Space
                        </button>
                    ` : userType === 'UserDashboard' ? `<button class="button button-secondary space-cancel-btn mt-2">
                        Cancel booking
                        </button>`: ''
        }
            </div>
        `)
    })
}


const createEventDataList = (gridSection, list, userType) => {
    gridSection.html("");
    
    list.forEach(event => {
        let startTime = new Date(event.startTime).toLocaleString();
        let endTime = new Date(event.endTime).toLocaleString();
        let description  = event.description.slice(0, 100);
        if (event.description.length > 100) {
            description += "..."
        }
 
        gridSection.append(`
            <div class="grid-item">
                <div class="img-icon">
                    <img alt="user" src="/icons/elementor.svg"/>
                </div>
                
                <div class="name my-2">${event.name}</div>
                <div class="description">
                    ${ description }
                </div>
                
                <div class="date opacity-75 mt-1">
                    Starts at ${ startTime }, Ends at ${ endTime }
                </div>
                
                <div class="data-row address my-2">
                    <span class="fw-bold mr-1">Address:</span>
                    ${ event.address }
                </div>
                <div class="data-row">
                    <span class="fw-bold mr-1">Ticket price:</span>
                    &#163; ${ event.ticketPrice }
                </div>
                
                <div class="data-row">
                    <span class="fw-bold">Total spaces:</span>
                    ${ event.totalSpaces }
                </div>
                
                
                <div class="data-row my-1">
                    <span class="fw-bold">Booked spaces:</span>
                    ${ event.bookedSpaces }
                </div>
                ${
            userType === 'Admin' ? `
                    <button class="button button-primary mt-3 event-edit-btn">
                        Edit Event
                    </button>
                    
                    <button class="delete-icon">
                        <box-icon class="icon" name="trash"></box-icon>
                    </button>

                ` : userType === 'EndUser' ? `
                    <button class="button button-primary mt-3 event-view-btn">
                        View Event
                     </button>
                    `: userType === 'UserDashboard' ? `<button class="button button-secondary event-cancel-btn mt-2">
                        Cancel booking
                        </button>`: ''
        }
            </div>
        `)
    })

}