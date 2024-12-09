let eventFields = [
    {
        label: "Name",
        name: "name",
        type: "text",
        value: "",
        placeholder: "Event name",
        hasError: false,
        errorText: "",
        isRequired: true,
        halfWidth: false
    },
    {
        label: "Description",
        name: "description",
        type: "textarea",
        value: "",
        placeholder: "Event description (50-300 characters long)",
        hasError: false,
        errorText: "",
        isRequired: true,
        halfWidth: false
    },
    {
        label: "Address",
        name: "address",
        type: "text",
        value: "",
        placeholder: "Address where event is taking place",
        hasError: false,
        errorText: "",
        isRequired: true,
        halfWidth: false
    },
    {
        label: "Total Available Spaces",
        name: "totalSpaces",
        type: "text",
        value: "",
        placeholder: "Available spaces for this event",
        hasError: false,
        errorText: "",
        isRequired: true,
        halfWidth: true
    },
    {
        label: "Ticket Price",
        name: "ticketPrice",
        type: "text",
        value: "",
        placeholder: "Event ticket price",
        hasError: false,
        errorText: "",
        isRequired: true,
        halfWidth: true
    },
    {
        label: "Start Date/Time",
        name: "startTime",
        type: "datetime-local",
        value: "",
        placeholder: "Event start date/time",
        hasError: false,
        errorText: "",
        isRequired: true,
        halfWidth: true
    },
    {
        label: "End Date/Time",
        name: "endTime",
        type: "datetime-local",
        value: "",
        placeholder: "Event end date/time",
        hasError: false,
        errorText: "",
        isRequired: true,
        halfWidth: true
    }
]

let spaceFields = [
    {
        label: "Room ID",
        name: "roomId",
        type: "text",
        value: "",
        placeholder: "Example: SC105",
        hasError: false,
        errorText: "",
        isRequired: true,
        halfWidth: true
    },
    {
        label: "Total Seats Available",
        name: "totalSeats",
        type: "text",
        value: "",
        placeholder: "Seats available in space",
        hasError: false,
        errorText: "",
        isRequired: true,
        halfWidth: true
    },
    {
        label: "Minimum membership level required",
        name: "minimumAccessLevel",
        type: "select",
        value: "Community",
        options: [
            { label: "Community", value: "Community" },
            { label: "Key Access", value: "KeyAccess" },
            { label: "Creative Workspace", value: "CreativeWorkspace" },
        ],
        placeholder: "Minimum membership required to access the space",
        hasError: false,
        errorText: "",
        isRequired: true,
        halfWidth: false
    },
    {
        label: "Opening Time",
        name: "openingTime",
        type: "time",
        value: "",
        placeholder: "Space opening time",
        hasError: false,
        errorText: "",
        isRequired: true,
        halfWidth: true
    },
    {
        label: "Closing Time",
        name: "closingTime",
        type: "time",
        value: "",
        placeholder: "Space closing time",
        hasError: false,
        errorText: "",
        isRequired: true,
        halfWidth: true
    }
]

const getFieldsWithErrors = (fieldArray) => {
    $(".create-action-modal .base-input").removeClass("with-error");
    $(".create-action-modal .field-item .error-text").text("");

    let fieldItems = document.querySelectorAll(".create-action-modal .base-input");
    let errorFields = document.querySelectorAll(".create-action-modal .field-item .error-text")
    return fieldArray.filter((field, index) => {
        if (field.isRequired && !field.value) {
            fieldItems[index].classList.add("with-error");
            errorFields[index].innerText = "This field is required.";
            return true;
        }

        if (field.hasError) {
            fieldItems[index].classList.add("with-error");
            errorFields[index].innerText = field.errorText;
        }
        return field.hasError;
    })
}


const createOptionList = (field) => {
    let options = ''
    field.options.forEach((option) => {
        let optionElement = document.createElement("option");
        optionElement.selected = option.value === field.value;
        optionElement.innerText = option.label;
        optionElement.value = option.value;
        options += optionElement.outerHTML;
    })
    
    return options;
}
const createFieldElements = (array) => {
    let fieldElement = "";
    let date = new Date();
    let minDate = `${date.getFullYear()}-${date.getMonth() + 1}-${date.getDate()}T${date.getHours()}:${date.getMinutes()}`;

    array.forEach(field => {
        let fieldItem  = `<div class="field-item${ field.halfWidth ? ' half-width': ''}">
            <label>${ field.label }</label>
            <div class="field-container">
             ${
            field.type === "textarea" ? `
                        <textarea 
                          
                            class="base-input"
                            placeholder="${ field.placeholder }" 
                            name="${ field.name }">${ field.value }</textarea>
                     ` : field.type === "text" ? `<input
                            name="${field.name}"
                            type="${field.type}"
                            value="${field.value}"
                            placeholder="${ field.placeholder }"
                            class="base-input"
                        />` : field.type === 'select' ?
                            `
                                <select class="base-input" name="${ field.name }">
                                    ${ createOptionList(field) }
                                </select>
                            `: `<input
                            name="${field.name}"
                            type="${field.type}"
                            value="${field.value}"
                            placeholder="${ field.placeholder }"
                            class="base-input"
                            min="${minDate}"
                        />`
        }
            </div>
            <span class="error-text">${ field.errorText }</span>
        </div>`

        fieldElement += fieldItem;
    })

    return fieldElement;
}
const createEventFields = () => {
   return createFieldElements(eventFields)
}

const createSpaceFields = () => {
    return createFieldElements(spaceFields)
}
const loadAdmin = async () => {
    let response = await fetch("/Admin", {
        method: "GET",
        headers: {
            Accept: 'application/json',
            'Content-Type': 'application/json',
        },
        credentials: 'include',
    })
    
    if (!response.ok) {
        console.log({ response })
        if (response.status === 401) {
            window.location.href = "/Admin/Login";
        }
        throw new Error(response.statusText);
    }
    
    let body = await response.json();
    
    return body.value;
}

const approveAccount = async (userId) => {
    let response = await fetch(`/Admin/Approve/${userId}`, {
        method: "POST",
        headers: {
            Accept: 'application/json',
            'Content-Type': 'application/json',
        },
        credentials: "include",
    })
    
    if (!response.ok) {
        console.log("Error response:", response);
        throw new Error(response.statusText);
    }
    
    return await response.json();
}
const loadUnapprovedAccounts = async () => {
    let response = await fetch(`/EndUser/Unapproved`, {
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
    
    let body = await response.json();
    return body;
}

const searchUnapprovedAccounts = async (query) => {
    let route = `/EndUser/Search?query=${query}&approved=false&hasMembership=true`
    let response = await fetch(route, {
        method: "GET",
        headers: {
            Accept: 'application/json',
            'Content-Type': 'application/json',
        },
        credentials: 'include',
    })

    if (!response.ok) {
        console.log({ response })
        throw new Error(response.statusText);
    }


    let body = await response.json();
    return body;
}
var systemAdmin, usersList = [];

const createUserList = () => {
    let userGrid = $(".user-grid");
    userGrid.html("");
    usersList.forEach(user => {
       
        let date= new Date(user.createdAt).toDateString();
        userGrid.append(`
        <div class="grid-item">
            <div class="img-icon">
                <img alt="user" src="/icons/user-icon.svg"/>
            </div>
            
            <div class="name my-2">${user.firstName } ${ user.lastName }</div>
            <div class="user-email">${ user.email }</div>
            <div class="user-phone mt-1">${ user.phone }</div>
            
            <div class="user-membership mt-3">${ user.membership.name } Membership</div>
            <div class="application-date mt-1">Applied on ${ date }</div>
            
            <button class="button button-secondary approve-btn mt-3">
                Approve
            </button>
        </div>
        `)
    })
}

const getUnapprovedAccounts = (query = "") => {
    let userErrorSection = $(".user-list-section .load-error-section");
    let userLoadSection = $(".user-list-section .loading-section");
    let userGrid = $(".user-list-section .user-grid");
    

    userErrorSection.addClass("d-none");
    userGrid.addClass("d-none");
    
    console.log("Fetching unapproved accounts");
    
    $(".empty-list-section").remove();

    userLoadSection.removeClass("d-none");
    let request = !query ? loadUnapprovedAccounts() : searchUnapprovedAccounts(query);
    request.then(list => {
        userLoadSection.addClass("d-none");
        userErrorSection.addClass("d-none");
        
        console.log({ list })
        userGrid.removeClass("d-none");
        usersList = [ ...list ];
        
        
        if (list.length === 0) {
            addEmptyListSection(!!query)
        }
        createUserList();
    }).catch(err => {
        console.log("Load error:", err)
        userErrorSection.removeClass("d-none");
        userLoadSection.addClass("d-none");



        $(".user-list-section .load-error-section .spinner + span").text(err.message);
    })
}

const addEmptyListSection = (passedQuery = false) => {
        $(".user-list-section .empty-list-section").remove();
       
        let grid = $(".user-list-section .user-grid")
        if (!grid.hasClass("d-none")) {
           grid.addClass("d-none");
        }
        
        $(".user-list-section").append(
            `<div class='empty-list-section text-center py-5'>
                ${ !passedQuery ? "There are currently no unapproved users." : "No unapproved users found." }
            </div>`
        )
}

const createActionModal = (index, type) => {
    let update = index !== null;
    let modal = $(`.modal.${type}-create-modal`);
    let actionLabel = type === 'event' ? "Event" : "Space"
    modal.remove();
    let modalBody = `
            <div class="modal create-action-modal ${type}-create-modal">
               <div class="modal-element">
                    <div class="modal-header">
                        <h3>${ update ? "Edit" : "Create" } ${ actionLabel }</h3>
                        <box-icon class="close-icon" name="x"></box-icon>
                    </div>
                     
                     <form method="post">
                        <div class="modal-body">
                        <div class="form-fields">
                          ${ type === 'event' ? createEventFields() : createSpaceFields() }
                        </div>
                     </div>
                    
                      <div class="modal-footer d-flex justify-content-center">
                        <button data-index="${ update ? index : -1 }" class="button button-secondary create-action-btn ${type}-action-btn">
                            ${ update ? "Edit" : "Create" } ${ actionLabel }
                        </button>
                      </div>
                    </form>
                </div>
            </div>
        `

    $("body").append(modalBody);
    setTimeout(function () {
        let modal = $(`.modal.${type}-create-modal`);
        modal.addClass("in-view");
        
    }, 0)
}
const createSpaceModal = (eventIndex = null) => {
    createActionModal(eventIndex, "space")
}
const createEventModal = (eventIndex = null) => {
    createActionModal(eventIndex, "event")
}
$(document).ready(function () {
    $(".breadcrumb .breadcrumb-item.sub").text(getBreadcrumbPage());
    
    let loadSection = $(".layout-load-section.process-section");
    let errorSection = $(".layout-load-section.error-section");
    let mainSection = $("main.main-container");
    
    loadAdmin()
        .then(response => {
            console.log("Response: ", response);
            loadSection.remove();
            errorSection.remove();
            
            
            systemAdmin = response;
            let userName = systemAdmin.firstName + " " + systemAdmin.lastName?.slice(0, 1);
            $(".main-header .user-section .user-name").text(userName);
            $(".header-section .user-welcome .user-name").text(systemAdmin.firstName);
            mainSection.removeClass("d-none");
            
            let path = window.location.pathname.slice(1).split(/\//g);
            if (path.length >= 2 && path[1] === 'Dashboard') {
                console.log({ path })
                getUnapprovedAccounts();
            }
           
        }).catch(error => {
            console.log("Error: ", error);
            loadSection.remove();
            errorSection.removeClass("d-none")
            $(".layout-load-section.error-section .load-text").text(error.message);
        })
    
    
    $(document).on("click", ".grid-item .approve-btn", function (event) {
          let index = $(".grid-item .approve-btn").index(this);
          console.log("Index:", index);
          
          let user = usersList[index];
          
          $(this).attr("disabled", "disabled");
          $(this).text("Approving account...")
          console.log({ user })
        
        approveAccount(user.id)
            .then(response => {
                console.log("Response:", response);
                $(this).removeClass("button-secondary");
                $(this).addClass("button-success");
                $(this).text("Account approved!");
                
                usersList = usersList.filter(item => item.id !== user.id);
                
                
                setTimeout(() => {
                    let gridItem = $(".user-grid .grid-item");
                    if (gridItem[index]) {
                        gridItem.addClass("transition");
                        setTimeout(() => {
                            gridItem.remove();
                            if (usersList.length === 0) {
                                addEmptyListSection()
                            }
                        }, 250)
                    }
                }, 3000)
            }).catch(err => {
                $(this).addClass("button-error");
                $(this).text("Error approving account");
                $(this).removeAttr("disabled");
                
                setTimeout(() => {
                    $(this).removeClass("button-error");
                    $(this).text("Approve");
                }, 3000)
            })
    })
    $("#accountSearch").on("input",debounce(function (event) {
        console.log("Input changed:", event.target.value)
        getUnapprovedAccounts(event.target.value)
    }, 1000))
    
    
    
    const closeModal = () => {
        let modal = $(".modal.create-action-modal");
        modal.removeClass("in-view");
        
        eventFields = eventFields.map(field => {
            field.value = ""
            return field;
        })
        
        setTimeout(function () {
           modal.remove(); 
        }, 350)
    }
    
    const createRequestBody = (array) => {
        let data = {};
        array.forEach(field => {
            data = {
                ...data,
                [field.name] : field.value,
            }
        })
        
        return data
    }
    const updateEvent = async (id) => {
        let response = await fetch(`/Event/${id}`, {
            method: "PUT",
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
            },
            credentials: 'include',
            body: JSON.stringify(createRequestBody(eventFields))
        })

        if (!response.ok) {
            throw new Error(response.statusText);
        }
        return await response.json();
    }
    
    
    const createEvent = async () => {
        let data = createRequestBody(eventFields);
        let response = await fetch(`/Event/Create`, {
            method: "POST",
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
            },
            credentials: "include",
            body: JSON.stringify(data)
        })
        
        console.log({ response })
        if (!response.ok) {
            throw new Error(response.statusText)
        }
        
        return await response.json();
    }
    
    const createSpace = async () => {
        let data = createRequestBody(spaceFields);
        let response = await fetch(`/Space/Create`, {
            method: "POST",
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
            },
            credentials: "include",
            body: JSON.stringify(data)
        })

        
        if (!response.ok) {
            throw new Error(response.statusText)
        }

        return await response.json();
    }
    
    
    const updateSpace = async (id) => {
        let data = createRequestBody(spaceFields);
        let response = await fetch(`/Space/${id}`, {
            method: "PUT",
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
            },
            credentials: 'include',
            body: JSON.stringify(data)
        })

        if (!response.ok) {
            throw new Error(response.statusText);
        }
        return await response.json();
    }
    $(document).on("click", ".event-create-btn", function (event) {
        event.preventDefault();
        createEventModal();
    })
    
    $(document).on("click", ".space-create-btn", function (event) {
        event.preventDefault();
        createSpaceModal()
    })
    
    $(document).on("input", ".create-action-modal .base-input", function (event) {
        let name = event.target.name;
        let value = event.target.value;
        let index = $(".create-action-modal .base-input").index(this);
        
        let errorField = $(".create-action-modal .field-item .error-text")[index];
        console.log({ errorField })
        
        console.log({
            name, value, index
        })
        let container = event.target.closest(".create-action-modal");
        console.log({ container })

        let array = container.classList.contains("space-create-modal") ? spaceFields : eventFields;
        array[index] = {
            ...array[index],
            value,
        }
        
        if (!value) {
          if (array[index].isRequired) {
              $(this).removeClass("with-error");
              let errorText =!value ? "This field is required" : ""
              array[index] = {
                  ...array[index],
                  hasError: !value,
                  errorText
              }

              errorField.innerText = errorText
              if (!value) {
                  $(this).addClass("with-error");
              }
          }
          
          
        } else {
            if (
                name === 'totalSpaces' 
                || name === 'totalSeats' 
                || name === "ticketPrice"
            ) {
                let value = name === 'totalSeats' ? parseInt(event.target.value) : parseFloat(event.target.value)
                console.log({ value })
                $(this).removeClass("with-error");
                array[index] = {
                    ...array[index],
                    hasError: isNaN(value),
                    errorText: isNaN(value) ? "Invalid value" : ""
                }
                errorField.innerText = ""
                if (isNaN(value)) {
                    $(this).addClass("with-error");
                    errorField.innerText = "Invalid value"
                }
            } else {
                errorField.innerText = "";
                $(this).removeClass("with-error");

                array[index] = {
                    ...array[index],
                    hasError: false,
                    errorText: ""
                }
            }
        }
       
        
        
        
        
    })
    $(document).on("click", ".create-action-modal .close-icon", function (event) {
        closeModal();
    })
    
    $(document).on("click", ".space-action-btn", function (event) {
        event.preventDefault();

        let index = parseInt($(this).attr("data-index"));
        let fieldsWithError = getFieldsWithErrors(spaceFields);
        const resetButton = () => {
            $(this).removeClass("button-error");
            $(this).removeClass("button-success");

            $(this).addClass("button-secondary");
            $(this).text(`${ index === -1 ? 'Create': 'Update' } Space`);

            $(this).removeAttr("disabled")
        }
        
        
       
        if (fieldsWithError.length === 0) {
            $(this).attr("disabled", "disabled");
            $(this).text(`${index === -1 ? 'Creating' : 'Updating'} space...`);

            let spaceItem = index !== -1 ? spaceList[index] : null;
            let request = index === -1 ? createSpace() : updateSpace(spaceItem?.id);
            request.then(space => {
                console.log("Space:", { space });
                $(this).removeClass("button-secondary");
                $(this).addClass("button-success");
                $(this).text(`Space ${ index === -1 ? 'Created' : 'Updated' }!`)

                if (spaceList) {
                    if (index !== -1) {
                        spaceList[index] = space;
                    } else {
                        spaceList.push(space)
                    }
                    createSpaceList();
                }
                
                setTimeout(() => {
                    resetButton();
                    closeModal();
                }, 2000);
            }).catch(err => {
                let message = err.message;
                console.log({ errorMessage: message })
                $(this).removeClass("button-secondary");
                $(this).addClass("button-error");
                $(this).text(`Error ${ index === -1 ? 'creating' : 'updating' } event`);

                setTimeout(() => {
                    resetButton();
                }, 3000);
            })
        }
    })
    
   
    $(document).on("click", ".event-action-btn", function (event) {
        event.preventDefault();

        let index = parseInt($(this).attr("data-index"));
        let fieldsWithError = getFieldsWithErrors(eventFields);
        const resetButton = () => {
            $(this).removeClass("button-error");
            $(this).removeClass("button-success");
            
            $(this).addClass("button-secondary");
            $(this).text(`${ index === -1 ? 'Create': 'Update' } Event`);
            
            $(this).removeAttr("disabled")
        }
        
        if (fieldsWithError.length === 0) {

            console.log({ index })
            $(this).attr("disabled", "disabled");
            $(this).text(`${index === -1 ? 'Creating' : 'Updating'} event...`)
            
           
            let eventItem = index !== -1 ? eventList[index] : null;
            console.log("Event:", event);
            let request = index === -1 ? createEvent() : updateEvent(eventItem?.id);
            
            request.then(event => {
                
                console.log("Event:", { event });
                $(this).removeClass("button-secondary");
                $(this).addClass("button-success");
                $(this).text(`Event ${ index === -1 ? 'Created' : 'Updated' }!`)
                
                if (eventList) {
                    if (index !== -1) {
                        eventList[index] = event;
                    } else {
                        eventList.push(event);
                    }

                    createEventList();
                }
              
                
                
                setTimeout(() => {
                    resetButton();
                    closeModal();
                }, 2000);
            }).catch(error => {
                let message = error.message;
                console.log({ errorMessage: message })
                $(this).removeClass("button-secondary");
                $(this).addClass("button-error");
                $(this).text(`Error ${ index === -1 ? 'creating' : 'updating' } event`);
                
                setTimeout(() => {
                    resetButton();
                }, 3000);
                
            })
        }
    })
})


