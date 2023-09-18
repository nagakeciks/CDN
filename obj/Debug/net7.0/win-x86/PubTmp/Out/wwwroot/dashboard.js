document.addEventListener('DOMContentLoaded', function () {

    var currDomain = window.location.origin;
    const token = sessionStorage.getItem('token');
    const userId = sessionStorage.getItem('userId');

    if (!token) {
        //redirect unauthorized user to the login page
       window.location.href = 'login.html';
    } 

    const headers = new Headers({
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json', 
    });
   
    getProfile();
    getUserList();

    $('.logout-button').on('click', function () {
        sessionStorage.removeItem('token');
        window.location.href = '/login.html'; 
    });


    function getUserList() {
        const apiUrl = `${currDomain}/api/User/GetUserList?UserID=${userId}`;

        return fetch(apiUrl, {
            method: 'GET',
            headers: headers,
            })
            .then(response => response.json())
            .then(data => {
                if (data.length > 0) {
                    populateUserList(data);
                }
            })
            .catch(error => console.error('Error fetching registration data:', error));
    }

    function getProfile() {
        const apiUrl = `${currDomain}/api/User/GetProfile?UserID=${userId}`;

        return fetch(apiUrl, {
            method: 'GET',
            headers: headers,
        })
            .then(response => response.json())
            .then(myProfile => {
                console.log(myProfile);
                document.getElementById("profile").innerHTML = `
            <div class="card-body">
                <h5 class="card-title">${myProfile.userName}</h5>
                <p class="card-text">Email: ${myProfile.mail}</p>
                <p class="card-text">Phone: ${myProfile.phoneNo}</p>
                <p class="card-text">Skills: ${myProfile.skillList}</p>
                <p class="card-text">Hobbies: ${myProfile.hobbyList}</p>
                
            </div>`; 
            })
            .catch(error => console.error('Error fetching profile:', error));
    }

    function populateUserList(userData) {

        const userListContainer = document.getElementById('userListContainer');
        userListContainer.innerHTML = "";
        // Add the header for the user list
        const userListHeader = document.createElement('h2');
        userListHeader.textContent = 'CDN User List';
        userListContainer.appendChild(userListHeader);

        const userList = document.createElement('div');

        // Loop through other users and display their information
        userData.forEach(user => {
            console.log(user);
            const userCard = document.createElement('div');
            userCard.className = 'card mb-3';
            userCard.innerHTML = `
                <div class="card-body">
                    <h5 class="card-title">${user.userName}</h5>
                    <p class="card-text">Email: ${user.mail}</p>
                    <p class="card-text">Phone: ${user.phoneNo}</p>
                    <p class="card-text">Status: ${user.status}</p>
                    <button class="btn btn-primary send-chat-button" data-user-id="${user.userID}" ${user.followedStatus === 'No' || user.status !== 'Online' ? 'disabled' : ''}>Send Chat</button>
                    <button class="btn btn-success follow-button" data-user-id="${user.userId}" data-followed-status="${user.followedStatus}">${user.followedStatus === 'Yes' ? 'Following' : 'Follow'}</button>
                </div>
            `;
            userList.appendChild(userCard);
        });

        userListContainer.appendChild(userList);



        $('.send-chat-button').on('click', function () {
            const ToID = $(this).data('user-id');
            openChatPopup(ToID);
        });

        $('.follow-button').on('click', function () {
            const userId = $(this).data('user-id');
            const followedStatus = $(this).data('followed-status');

            if (followedStatus === 'No') {
                followUser(userId);
            } else {
                unfollowUser(userId);
            }
        });
    }



    function openChatPopup(ToID) {

        const chatDialog = $('<div></div>')
            .html('<p>Enter your chat message:</p><textarea id="chatMessage" rows="4" cols="50"></textarea>')
            .dialog({
                title: 'Send Chat',
                width: 600, 
                height: 291,
                modal: true,
                buttons: {
                    'Send': function () {
                        const chatMessage = $('#chatMessage').val();
                        sendChatMessage(ToID, chatMessage);
                        $(this).dialog('close');
                    },
                    'Cancel': function () {
                        $(this).dialog('close');
                    },
                },
                close: function () {
                    $(this).dialog('destroy').remove();
                },
            });
    }


    function sendChatMessage(ToID, message) {
        sendMessageFrom(userId, ToID, message);
        console.log(`Chat message sent to User ${ToID}: ${message}`);

    }

    function sendMessageFrom(FromID, ToID, message) {
        console.log(`message from : ${FromID}`);
        console.log(`message to : ${ToID}`);
        console.log(message);
        connection.invoke("SendMessageToUser", parseInt(FromID), parseInt(ToID), message).catch(function (err) {
            console.error(err.toString());
        });
    }



    function followUser(userId) {

        const apiUrl = `HafizMightAddLater/${userId}`;

        fetch(apiUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',

            },
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error(`Error following user: ${response.status}`);
                }
                return response.json();
            })
            .then(data => {
                // Handle a successful follow action
                console.log(`Following User ${userId}`);
                // Update the "Follow" button to indicate following
                $(`.follow-button[data-user-id="${userId}"]`).text('Following').addClass('btn-success');
                $(`.follow-button[data-user-id="${userId}"]`).data('followed-status', 'Yes');
                // Enable the "Send Chat" button
                $(`.send-chat-button[data-user-id="${userId}"]`).prop('disabled', false);
            })
            .catch(error => {
                // Handle errors from the API
                console.error(error);
            });
    }


    function unfollowUser(userId) {

        const apiUrl = `HafizMightAddLater/${userId}`;

        fetch(apiUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error(`Error unfollowing user: ${response.status}`);
                }
                return response.json();
            })
            .then(data => {
                // Handle a successful unfollow action
                console.log(`Unfollowing User ${userId}`);
                // Update the "Follow" button to indicate not following
                $(`.follow-button[data-user-id="${userId}"]`).text('Follow').removeClass('btn-success');
                $(`.follow-button[data-user-id="${userId}"]`).data('followed-status', 'No');
                // Disable the "Send Chat" button
                $(`.send-chat-button[data-user-id="${userId}"]`).prop('disabled', true);
            })
            .catch(error => {
                // Handle errors from the API
                console.error(error);
            });
    }


    function displayReceivedChatMessage(from, message) {
        // Create the chat dialog
        const chatDialog = $('<div></div>')
            .html(`
            <p><strong>From:</strong> ${from}</p>
            <p><strong>Message:</strong> ${message}</p>
        `)
            .dialog({
                title: 'Received Chat',
                width: 400, // Set the width as needed
                height: 200, // Set the height as needed
                modal: true,
                buttons: {
                    'Close': function () {
                        $(this).dialog('close');
                    },
                },
                close: function () {
                    $(this).dialog('destroy').remove();
                },
            });
    }


    function receiveChatMessage(from,message) {
        displayReceivedChatMessage(from, message);
    }



    var ConnID = "";
    // http://hafizsalam.sytes.net/signalr/chat-hub
    // https://hafiz-cdn.azurewebsites.net/chat-hub
    // http://localhost:5083/chat-hub
    const connection = new signalR.HubConnectionBuilder()

        .withUrl(`${currDomain}/chat-hub`) 

        .build();

    connection.start().then(function () {
        console.log("Connected to the hub.");
    }).catch(function (err) {
        console.error(err.toString());
    });

    connection.on("ReturnID", function (message) {
        ConnID = message;
        console.log("Return ID: " + ConnID);
        RegisterUser();

    });

    function RegisterUser() {
        connection.invoke("MapCDNUserID", ConnID, parseInt(userId)).catch(function (err) {
            console.error(err.toString());
        });

    }

    connection.on("RefreshUserList", function () {
        console.log("RefreshUserList");
        getUserList();

    });

    connection.on("ReceiveMessage", function (message) {
        console.log("Received notification: " + message);
    });

    connection.on("ReceiveMessageFromID", function (from,message) {
        console.log("Received notification: " + message);
        receiveChatMessage(from, message);
    });


});
