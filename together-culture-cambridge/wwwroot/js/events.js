

const loadEvents = async () => {
    let response = await fetch("/Event", {
        method: "GET",
        headers: {
            Accept: 'application/json',
            'Content-Type': 'application/json',
        },
        credentials: 'include',
    })


    if (!response.ok) {
        throw new Error(response.statusText);
    }

    return await response.json();
}
const searchEvents = async (query) => {
    let response = await fetch(`/Event/Search?query=${query}`, {
        method: "GET",
        headers: {
            Accept: 'application/json',
            'Content-Type': 'application/json',
        },
        credentials: 'include',
    })

    if (!response.ok) {
        throw new Error(response.statusText);
    }

    return await response.json();
}

let eventList = [];
let path = location.pathname.slice(1).split(/\//g);
console.log(path);
let userType = path[0];
const createEventList = () => {
    let gridSection = $(".event-list-section .event-grid");
    createEventDataList(gridSection, eventList, userType)
}
const addEmptySection = () => {
    $(".event-list-section .empty-section").remove();

    let gridSection = $(".event-list-section .event-grid");
    if (!gridSection.hasClass("d-none")) {
        gridSection.addClass("d-none");
    }
    
    $(".event-list-section").append(
        `<div class="empty-list-section text-center py-3">
            No events found
        </div>`
    )
}
const getEvents = (query = "") => {
    let errorSection = $(".event-list-section .load-error-section");
    let loadingSection = $(".event-list-section .loading-section");
    let gridSection = $(".event-list-section .event-grid");
    
    
    errorSection.addClass("d-none");
    gridSection.addClass("d-none");
    
    loadingSection.removeClass("d-none");
    $(".event-list-section .empty-section").remove();
    
    let request = !query ? loadEvents() : searchEvents(query);
   request.then(events => {
        errorSection.addClass("d-none");
        loadingSection.addClass("d-none");
        
        gridSection.removeClass("d-none");
        console.log("Events:", events);
        
        
        eventList = [...events]
        
        if (events.length === 0) {
            addEmptySection()
        } else {
            createEventList();
        }
        
    }).catch(err => {
        console.log("Error:", err)
        errorSection.removeClass("d-none");
        loadingSection.addClass("d-none")

        $(".event-list-section .load-error-section .load-text").text(err.message);
        
    })
}
const closeEventBookingModal = () => {
    let modal = $(".modal.event-booking-modal");
    modal.removeClass("in-view");
    

    setTimeout(function () {
        modal.remove();
    }, 350)
}
const createEventBookingModal = (index) => {
    let event = eventList[index];
    $(".event-booking-modal").remove()
    let startTime = new Date(event.startTime).toLocaleString();
    let endTime = new Date(event.endTime).toLocaleString();
    
    let slotBooked = systemUser['eventBookings'].some(e => e.id === event.id);
    let modalBody = `
        <div class="modal event-booking-modal">
            <div class="modal-element">
                <div class="modal-header">
                    <h3>${event.name}</h3>
                    <box-icon class="close-icon" name="x"></box-icon>
                </div>
                
                <div class="modal-body">
                    <div>
                        <h5>Event description</h5>
                        <p class="opacity-75 mt-2">${event.description}</p>
                    </div>
                    <div class="my-2 event-date">
                        Starts at ${startTime}, ends at ${endTime}
                    </div>
                    <div class="my-4">
                        <h5>Event address</h5>
                         <p class="opacity-75">${event.address}</p>
                    </div>
                    
                    <div class="d-flex align-items-center justify-content-between">
                        <div class="data-row">
                            <span class="fw-bold mr-1">Ticket price:</span>
                            &#163; ${event.ticketPrice}
                        </div>
                
                        <div class="data-row">
                            <span class="fw-bold">Total spaces:</span>
                            ${event.totalSpaces}
                        </div>
                        
                
                        <div class="data-row my-1">
                            <span class="fw-bold">Booked spaces:</span>
                            ${event.bookedSpaces}
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="d-flex w-100  justify-content-center">
                       ${
                            !slotBooked ? `<button data-index="${index}" class="button button-secondary booking-btn">Book Slot</button>`
                                : `<button data-index="${index}" class="button button-secondary booking-cancel-btn">Cancel booking</button>`
                        }
                    </div>
                 </div>
            </div>
        </div>
    `

    $("body").append(modalBody);
    setTimeout(function () {
        let modal = $(`.modal.event-booking-modal`);
        modal.addClass("in-view");

    }, 0)
}
const deleteEvent = async (id) => {
    let response = await fetch(`/Event/Delete/${id}`, {
        method: "DELETE",
        headers: {
            Accept: 'application/json',
            'Content-Type': 'application/json',
        },
        credentials: 'include',
    })

    if (!response.ok) {
        throw new Error(response.statusText);
    }

    return await response.json();
}
$(document).ready(function () {
    getEvents();
    
    const runEventAction = (button, type) => {
        let index = parseInt(button.attr("data-index"));
      
        let eventId = eventList[index].id;

        const resetButton = () => {
            button.removeClass("button-error");
            button.removeClass("button-success");

            button.addClass("button-secondary");
            button.text(type === "booking" ? "Book Slot": "Cancel Booking");

            button.removeAttr("disabled")
        }

        button.attr("disabled", "disabled");
        button.text( type === 'booking' ? "Booking slot..." : "Cancelling booking...");

        let request = type === "booking" ? bookEventSlot(eventId) : cancelEventBooking(eventId);
        request.then(event => {
            button.removeClass("button-secondary");
            button.addClass("button-success");
            if (type === "booking") {
                button.text("Event slot booked!");

                systemUser['eventBookings'].push(event);
                eventList[index] = event;
               
            } else {
                button.text("Booking cancelled");
                systemUser['eventBookings'] = systemUser['eventBookings'].filter(e => e.id !== eventId);
                eventList[index].bookedSpaces -= 1;
            }

            createEventList()

            setTimeout(() => {
                resetButton();
                closeEventBookingModal();
            }, 2000)
        }).catch(err => {
            let message = err.message;
            console.log({ errorMessage: message })
            button.removeClass("button-secondary");
            button.addClass("button-error");
            button.text(err.message);

            setTimeout(() => {
                resetButton();
            }, 3000);
        })
    }
    $(".event-list-section #eventSearch").on("input",debounce(function (event) {
        let query = event.target.value
        getEvents(query);
    }, 1000))

    $(document).on("click", ".event-booking-modal .close-icon", function (event) {
        closeEventBookingModal();
    })
    
    $(document).on("click", ".event-booking-modal .booking-btn", function (event) {
        event.preventDefault();
        runEventAction($(this), "booking")
    })

    $(document).on("click", ".event-booking-modal .booking-cancel-btn", function (event) {
        event.preventDefault();
        runEventAction($(this), "cancellation")
    })
    $(document).on("click", ".event-grid .event-edit-btn", function (event) {
        let index = $(".event-grid .event-edit-btn").index(this);
        console.log("Clicked")
       
        if (userType === "Admin") {
            console.log("Index", index);
           
            let event = eventList[index];
            console.log({ event })
            
            eventFields = eventFields.map(field => {
                let value = event[field.name]
               
                field.value = value ? value : "";
                
                return field
            })

            console.log("Event fields:", eventFields)
            createEventModal(index)
        }
    })
    

    $(document).on("click", ".event-grid .event-view-btn", function (event) {
        let index = $(".event-grid .event-view-btn").index(this);
        createEventBookingModal(index)
    })
    $(document).on("click", ".event-grid .delete-icon", function (event) {
        let index = $(".event-grid .delete-icon").index(this);
        $(".delete-popup").remove();
        if (userType === "Admin") {
           $("body").append(`<div class="popup delete-popup d-flex justify-content-center align-items-center">
                <div class="spinner alt-spinner"></div>
                <span class="mx-2">Deleting event...</span>
            </div>`)
            
            
            let event = eventList[index];
            deleteEvent(event.id).then(() => {
                $(".delete-popup").remove();
                console.log("Event deleted")
                eventList = eventList.filter((e, eIndex) => eIndex !== index);
                createEventList()
            }).catch(err => {
                console.log("Error:", err);
                $(".delete-popup .spinner").remove();
                $(".delete-popup span").text("Error deleting event");
                setTimeout(function () {
                    $(".delete-popup").remove();
                }, 2000)
            })
        }
    })
})