
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
    gridSection.html("");

        
    
    eventList.forEach(event => {
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

` : ""
                 }
            </div>
        `)
    })
    
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


$(document).ready(function () {
    getEvents();
    $(".event-list-section #eventSearch").on("input",debounce(function (event) {
        let query = event.target.value
        getEvents(query);
    }, 1000))
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
    $(document).on("click", ".event-grid .delete-icon", function (event) {
        console.log("Delete event:", event);
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