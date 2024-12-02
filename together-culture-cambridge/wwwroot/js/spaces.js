let spaceList = [];
let path = location.pathname.slice(1).split(/\//g);
console.log(path);
let userType = path[0];
let bookingTime;

const loadSpaces = async () => {
    let response = await fetch("/Space", {
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


const searchSpaces = async (query) => {
    let response = await fetch(`/Space/Search?query=${query}`, {
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

const addEmptySpaceListSection = () => {
    $(".space-list-section .empty-section").remove();

    let gridSection = $(".space-list-section .space-grid");
    if (!gridSection.hasClass("d-none")) {
        gridSection.addClass("d-none");
    }

    $(".space-list-section").append(
        `<div class="empty-list-section text-center py-3">
            No spaces found
        </div>`
    )
}


const createSpaceList = () => {
    let gridSection = $(".space-list-section .space-grid");
   
    createSpaceDataList(gridSection, spaceList, userType)
   

}
const getSpaces = (query = "") => {
    let errorSection = $(".space-list-section .load-error-section");
    let loadingSection = $(".space-list-section .loading-section");
    let gridSection = $(".space-list-section .space-grid");

    errorSection.addClass("d-none");
    gridSection.addClass("d-none");
    
    loadingSection.removeClass("d-none");
    $(".space-list-section .empty-list-section").remove();
    
    let request = !query ? loadSpaces() : searchSpaces(query);
    request.then(spaces => {
        errorSection.addClass("d-none");
        loadingSection.addClass("d-none");

        gridSection.removeClass("d-none");
        console.log("Spaces:", spaces);
        
        spaceList = [ ...spaces ];
        if (spaces.length === 0) {
            addEmptySpaceListSection()
        } else {
            createSpaceList();
        }
    }).catch(err => {
        console.log("Error:", err)
        errorSection.removeClass("d-none");
        loadingSection.addClass("d-none")

        $(".space-list-section .load-error-section .load-text").text(err.message);
    })
}

const deleteSpace = async (id) => {
    let response = await fetch(`/Space/Delete/${id}`, {
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

const closeSpaceBookingModal = () => {
    let modal = $(".modal.space-booking-modal");
    modal.removeClass("in-view");


    setTimeout(function () {
        modal.remove();
    }, 350)
}


const createSpaceBookingModal = (index) => {
    let space = spaceList[index];
    $(".space-booking-modal").remove();

    let closingTime = getTimeString(space.closingTime);
    let openingTime = getTimeString(space.openingTime);
    
    let date =  new Date();
    let minDate = `${date.getFullYear()}-${date.getMonth() + 1}-${date.getDate()}T${date.getHours() + 1}:${date.getMinutes()}`;
    let spaceBooked = systemUser['spaceBookings'].find(e => e.id === space.id);
    let bookingDate = spaceBooked?.bookingDate ? new Date(spaceBooked?.bookingDate).toLocaleString() : ''
    console.log({ spaceBooked })
    let modalBody = `
        <div class="modal space-booking-modal">
            <div class="modal-element">
             <div class="modal-header">
                    <h3>${space['roomId']}</h3>
                    <box-icon class="close-icon" name="x"></box-icon>
              </div>
              
              <div class="modal-body d-flex flex-column align-items-center text-center">
                <div class="date opacity-75 mt-1">
                    Opens at ${ openingTime }, Closes at ${ closingTime }
                </div>
               
                <div class="my-2">
                 ${ space['bookedSeats'] } booked seats today, ${ space['totalSeats'] } total seats
                </div>
                
                ${
                     !spaceBooked ? `<div class="field-item  mt-2">
                        <label class="mb-2">Booking date/time</label>
                        <input
                            name="bookingDate"
                            type="datetime-local"
                            min="${minDate}"
                            placeholder="Book this space"
                            class="base-input booking-date"
                        />
                    </div>` : bookingDate ? `You have a slot booked for ${bookingDate}` : ''
                 }
              </div>
              
              <div class="modal-footer">
                    <div class="d-flex w-100  justify-content-center">
                       ${
                            !spaceBooked ? `<button data-index="${index}" class="button button-secondary booking-btn">Book space</button>`
                                : `<button data-index="${index}" class="button button-secondary booking-cancel-btn">Cancel booking</button>`
                        }
                    </div>
                 </div>
            </div>
        </div>
    `

    $("body").append(modalBody);
    setTimeout(function () {
        let modal = $(`.modal.space-booking-modal`);
        modal.addClass("in-view");

    }, 0)
}
$(document).ready(function () {
    getSpaces();
    const runSpaceAction = (button, type) => {
        let index = parseInt(button.attr("data-index"));
        let spaceId = spaceList[index].id;

        const resetButton = () => {
            button.removeClass("button-error");
            button.removeClass("button-success");

            button.addClass("button-secondary");
            button.text(type === "booking" ? "Book Space": "Cancel Booking");

            button.removeAttr("disabled")
        }
        button.attr("disabled", "disabled");
        button.text( type === 'booking' ? "Booking space..." : "Cancelling booking...");
        
        let request = type === 'booking' ? bookSlotAtSpace(spaceId, bookingTime) : cancelSpaceBooking(spaceId);
        request.then(space => {
            button.removeClass("button-secondary");
            button.addClass("button-success");
            if (type === "booking") {
                button.text("Space slot booked!");

                systemUser['spaceBookings'].push(space);
                spaceList[index] = space;

            } else {
                button.text("Booking cancelled");
                systemUser['spaceBookings'] = systemUser['spaceBookings'].filter(e => e.id !== spaceId);
                spaceList[index].bookedSpaces -= 1;
            }

            createSpaceList()

            setTimeout(() => {
                resetButton();
                closeSpaceBookingModal();
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
    $(".space-list-section #spaceSearch").on("input", debounce(function (event) {
        let query = event.target.value
        getSpaces(query);
    }, 1000))
    $(document).on("click", ".space-grid .space-edit-btn", function (event) {
        let index = $(".space-grid .space-edit-btn").index(this);
        if (userType === "Admin") {
            let space = spaceList[index];
            
            spaceFields = spaceFields.map(field => {
                let value = space[field.name]
                if (field.name.toLowerCase().includes("time") && value) {
                    let dateValue = new Date(value);
                    value  =`${dateValue.getHours()}:${dateValue.getMinutes()}`;
                }
                field.value = value ? value : "";
                return field
            })
            
            createSpaceModal(index)
        }
    })
    $(document).on("input", ".space-booking-modal .booking-date", function (event) {
        bookingTime = event.target.value;
        if (!bookingTime) {
            $(this).addClass("with-error");
        } else {
            $(this).removeClass("with-error");
        }
    })
    $(document).on("click", ".space-booking-modal .booking-btn", function () {
        let bookingField = $(".space-booking-modal .booking-date");
        bookingField.removeClass("with-error");
        
        
        if (!bookingTime) {
            bookingField.addClass("with-error");
        } else {
            runSpaceAction($(this), "booking");
        }
    })
    
    $(document).on("click", ".space-booking-modal .booking-cancel-btn", function () {
        runSpaceAction($(this), "cancellation");
    })
    
    

    $(document).on("click", ".space-grid .space-view-btn", function () {
        let index = $(".space-grid .space-view-btn").index(this);
        createSpaceBookingModal(index);
    })

    $(document).on("click", ".space-booking-modal .close-icon", function (event) {
        closeSpaceBookingModal();
    })
    
    $(document).on("click", ".space-grid .delete-icon", function () {
        let index = $(".space-grid .delete-icon").index(this);
       
        
        if (userType === "Admin") {
            $(".delete-popup").remove();
            $("body").append(`<div class="popup delete-popup d-flex justify-content-center align-items-center">
                <div class="spinner alt-spinner"></div>
                <span class="mx-2">Deleting space...</span>
            </div>`)
            
            let space = spaceList[index];
            deleteSpace(space.id).then(() => {
                $(".delete-popup").remove();
                spaceList = spaceList.filter((s, i) => i !== index);
                createSpaceList();
            }).catch(err => {
                console.log("Error:", err);
                $(".delete-popup .spinner").remove();
                $(".delete-popup span").text("Error deleting space");
                setTimeout(function () {
                    $(".delete-popup").remove();
                }, 2000)
            })
        }
    })
})