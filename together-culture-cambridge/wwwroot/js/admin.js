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


const createEventFields = () => {
    let fieldElement = "";
    let date = new Date();
    let minDate = `${date.getFullYear()}-${date.getMonth() + 1}-${date.getDate()}T${date.getHours()}:${date.getMinutes()}`;
    
    eventFields.forEach(field => {
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
                        />` : `<input
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

const createEventModal = (eventIndex = null) => {
    let update = eventIndex != null;
    
    let modal = $(".modal.event-create-modal");
    modal.remove();
    let modalBody = `
            <div class="modal event-create-modal">
               <div class="modal-element">
                    <div class="modal-header">
                        <h3>${ update ? "Edit" : "Create" } Event</h3>
                        <box-icon class="close-icon" name="x"></box-icon>
                    </div>
                     
                     <form method="post">
                        <div class="modal-body">
                        <div class="form-fields">
                          ${ createEventFields() }
                        </div>
                     </div>
                    
                      <div class="modal-footer d-flex justify-content-center">
                        <button data-index="${ update ? eventIndex : -1 }" class="button button-secondary create-action-btn">
                            ${ update ? "Edit" : "Create" } Event
                        </button>
                      </div>
                    </form>
                </div>
            </div>
        `

    $("body").append(modalBody);
    setTimeout(function () {
        let modal = $(".modal.event-create-modal");
        modal.addClass("in-view");
        
        console.log("Events:", eventList)
    }, 0)
}
$(document).ready(function () {
    let href = window.location.href;
    let hrefSplit = href.split('/');
    let lastSplitValue = hrefSplit[hrefSplit.length - 1];
    $(".breadcrumb .breadcrumb-item.sub").text(lastSplitValue);
    
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
            
            
           
            getUnapprovedAccounts();
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
        let modal = $(".modal.event-create-modal");
        modal.removeClass("in-view");
        
        eventFields = eventFields.map(field => {
            field.value = ""
            return field;
        })
        
        setTimeout(function () {
           modal.remove(); 
        }, 350)
    }
    
    const createRequestBody = () => {
        let data = {};
        eventFields.forEach(field => {
            data = {
                ...data,
                [field.name] : field.value,
            }
        })
        
        return data
    }
    const createEvent = async () => {
        let data = createRequestBody();
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
    $(document).on("click", ".event-create-btn", function (event) {
        event.preventDefault();
        console.log("Clicked")
        
        createEventModal();
    })
    
    $(document).on("input", ".event-create-modal .base-input", function (event) {
        let name = event.target.name;
        let value = event.target.value;
        let index = $(".event-create-modal .base-input").index(this);
        
        let errorField = $(".event-create-modal .field-item .error-text")[index];
        console.log({ errorField })
        
        console.log({
            name, value, index
        })

        eventFields[index] = {
            ...eventFields[index],
            value,
        }
        
        if (!value) {
          if (eventFields[index].isRequired) {
              $(this).removeClass("with-error");
              let errorText =!value ? "This field is required" : ""
              eventFields[index] = {
                  ...eventFields[index],
                  hasError: !value,
                  errorText
              }

              errorField.innerText = errorText
              if (!value) {
                  $(this).addClass("with-error");
              }
          }
          
          
        } else {
            if (name === 'totalSpaces' || name === "ticketPrice") {
                let value = parseFloat(event.target.value)
                console.log({ value })
                $(this).removeClass("with-error");
                eventFields[index] = {
                    ...eventFields[index],
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

                eventFields[index] = {
                    ...eventFields[index],
                    hasError: false,
                    errorText: ""
                }
            }
        }
       
        
        
        
        
    })
    $(document).on("click", ".event-create-modal .close-icon", function (event) {
        closeModal();
    })
    $(document).on("click", ".create-action-btn", function (event) {
        event.preventDefault();
        $(".event-create-modal .base-input").removeClass("with-error");
        $(".event-create-modal .field-item .error-text").text("");
        
        let fieldItems = document.querySelectorAll(".event-create-modal .base-input");
        let errorFields = document.querySelectorAll(".event-create-modal .field-item .error-text")
        let fieldsWithError = eventFields.filter((field, index) => {
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
        
        const updateEvent = async (id) => {
            let response = await fetch(`/Event/${id}`, {
                method: "PUT",
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                },
                credentials: 'include',
                body: JSON.stringify(createRequestBody())
            })
            
            if (!response.ok) {
                throw new Error(response.statusText);
            }
            return await response.json();
        }
        const resetButton = () => {
            $(this).removeClass("button-error");
            $(this).removeClass("button-success");
            
            $(this).addClass("button-secondary");
            $(this).text("Create Event");
            
            $(this).removeAttr("disabled")
        }
        
        if (fieldsWithError.length === 0) {
            let index = parseInt($(this).attr("data-index"));
            console.log({ index })
            $(this).attr("disabled", "disabled");
            $(this).text(`${index === -1 ? 'Creating' : 'Updating'} event...`)
            
           
            let event = eventList[index];
            console.log("Event:", event);
            let request = index === -1 ? createEvent() : updateEvent(event.id);
            
            request.then(event => {
                
                console.log("Event:", { event });
                $(this).removeClass("button-secondary");
                $(this).addClass("button-success");
                $(this).text(`Event ${ index === -1 ? 'Created' : 'Updated' }!`)
                
                eventList[index] = event;
                createEventList();
                
                
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


