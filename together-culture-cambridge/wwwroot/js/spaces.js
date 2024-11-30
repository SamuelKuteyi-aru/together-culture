let spaceList = [];
let path = location.pathname.slice(1).split(/\//g);
console.log(path);
let userType = path[0];

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

const createTimeItemString = (value) => {
    return value.toString().length === 1 ? `0${value}` : value.toString()
}
const getTimeString = (time) => {
    let dateTime = new Date(time)
    return createTimeItemString(dateTime.getHours()) + ":" + createTimeItemString(dateTime.getMinutes());
}
const createSpaceList = () => {
    let gridSection = $(".space-list-section .space-grid");
    gridSection.html("");
    
    spaceList.forEach(space => {


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
                    ${ space.totalSeats - space.bookedSeats } seats open
                </div>
                
                ${
                    userType === 'Admin' ? 
                        `<button class="button button-secondary space-edit-btn">
                                Edit Space
                        </button>
                         
                         
                        <button class="delete-icon">
                            <box-icon class="icon" name="trash"></box-icon>
                        </button>
                    ` : ''
                 }
            </div>
        `)
    })
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
$(document).ready(function () {
    getSpaces();
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
    
    $(document).on("click", ".space-grid .delete-icon", function (event) {
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