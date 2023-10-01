document.addEventListener('DOMContentLoaded', function () {

    var currDomain = window.location.origin;
    const token = sessionStorage.getItem('token');
    const userId = sessionStorage.getItem('userId');
    // Set the number of items per page
    var itemsPerPage = 2;
    var currentPage = 1;
    var maxPage = 10;



    if (!token) {
        //redirect unauthorized user to the login page
       window.location.href = 'login.html';
    } 

    const headers = new Headers({
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json', 
    });
   
    getProfile();
    //getUserListPaging();

    $('.logout-button').on('click', function () {

        const apiUrl = `${currDomain}/api/User/LogOut?UserID=${userId}&ConnID=${ConnID}`;

        return fetch(apiUrl, {
            method: 'GET',
            headers: headers,
        })
            .then(response => response.json())
            .then(data => {
                    sessionStorage.removeItem('token');
                    window.location.href = '/login.html'; 
            })
            .catch(error => console.error('Error fetching registration data:', error));
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
                    //reatePaging();
                    populateUserList(data);
                }
            })
            .catch(error => console.error('Error fetching registration data:', error));
    }

    function getUserListPaging() {
        const apiUrl = `${currDomain}/api/User/GetUserListPaging?UserID=${userId}&PageSize=${itemsPerPage}&PageNum=${currentPage}`;

        return fetch(apiUrl, {
            method: 'GET',
            headers: headers,
        })
            .then(response => response.json())
            .then(data => {
                console.log(data);
                if (data.maxSize > 0) {
                    //CreatePaging();
                    maxPage = data.maxSize;
                    populateUserList(data.userList);
                }
            })
            .catch(error => console.error('Error fetching registration data:', error));
    }

    // Function to display items for the current page
    function displayItems() {
        document.getElementById('pageNum').innerHTML = currentPage;
    }

    // Initial display
    displayItems();

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

    $('.next-button').on('click', function () {

        if (currentPage < maxPage) {
            currentPage++;
            getUserListPaging();
            displayItems();
        }
        if (currentPage == maxPage) {return; }
    });

    $('.previous-button').on('click', function () {
        if (currentPage == 1) { return; }
        if (currentPage > 1) {
            currentPage--;
            getUserListPaging();
            displayItems();
        }
        if (currentPage < maxPage) { $('.next-button').css("display", "inline"); }
        
    });

    //function CreatePaging() {
    //    const userListContainer = document.getElementById('userListPage');
    //    const Paging = document.createElement('div');
    //    /*        Paging.id = "Pagination";*/
    //    userListContainer.innerHTML = '';
    //    Paging.innerHTML = `
    //        <button class="btn btn-link previous-button">Previous</button>
    //        <label for='pagenumber'>Page Number : </label> <span id='pageNum'>1</span>
    //        <button class="btn btn-link next-button">Next</button>`;
    //    userListContainer.appendChild(Paging);

    //    $('.next-button').on('click', function () {
    //        if (currentPage < maxPage) {
    //            currentPage++;
    //            displayItems();
    //        }
    //    });

    //    $('.previous-button').on('click', function () {
    //        if (currentPage > 1) {
    //            currentPage--;
    //            displayItems();
    //        }
    //    });
    //}
    function populateUserList(userData) {

        const userListContainer = document.getElementById('userListDyn');
        userListContainer.innerHTML = "";
        
        // Add the header for the user list
        const userListHeader = document.createElement('h2');
        userListHeader.textContent = 'CDN User List';
        userListContainer.appendChild(userListHeader);

        const listUser = document.getElementById('listUser');
        listUser.innerHTML = "";
        userData.forEach(user => {
            console.log(user);
            var strUser = `
                    <li class="list-group-item">
                    Username : ${user.userName} <br>
                    Email: ${user.mail} <br>
                    Phone: ${user.phoneNo} <br>
                    Status: ${user.status} <br>
                    <button class="btn btn-primary send-chat-button" data-user-id="${user.userID}" ${user.followedStatus === 'No' || user.status !== 'Online' ? 'disabled' : ''}>Send Chat</button>
                    <button class="btn btn-success follow-button" data-user-id="${user.userId}" data-followed-status="${user.followedStatus}">${user.followedStatus === 'Yes' ? 'Following' : 'Follow'}</button></li>
            `;
            listUser.innerHTML += strUser;
            //listUser.appendChild(strUser);
        });

//        const userList = document.createElement('div');

//        // Loop through other users and display their information
//        userData.forEach(user => {
//            console.log(user);
//            const userCard = document.createElement('div');
//            userCard.className = 'card mb-3';
//            userCard.innerHTML = `
//                <!--div class="card-body">
//                    <h5 class="card-title">${user.userName}</h5>
//                    <p class="card-text">Email: ${user.mail}</p>
//                    <p class="card-text">Phone: ${user.phoneNo}</p>
//                    <p class="card-text">Status: ${user.status}</p>
//                    <button class="btn btn-primary send-chat-button" data-user-id="${user.userID}" ${user.followedStatus === 'No' || user.status !== 'Online' ? 'disabled' : ''}>Send Chat</button>
//                    <button class="btn btn-success follow-button" data-user-id="${user.userId}" data-followed-status="${user.followedStatus}">${user.followedStatus === 'Yes' ? 'Following' : 'Follow'}</button>
//                </div-->
//<ul class="list-group">
//                    <li class="list-group-item">${user.userName}</li>
//                    <li class="list-group-item">Email: ${user.mail}</li>
//                    <li class="list-group-item">Phone: ${user.phoneNo}</li>
//                    <li class="list-group-item">Status: ${user.status}</li>
//                    <li class="list-group-item"><button class="btn btn-primary send-chat-button" data-user-id="${user.userID}" ${user.followedStatus === 'No' || user.status !== 'Online' ? 'disabled' : ''}>Send Chat</button></li>
//                    <li class="list-group-item"><button class="btn btn-success follow-button" data-user-id="${user.userId}" data-followed-status="${user.followedStatus}">${user.followedStatus === 'Yes' ? 'Following' : 'Follow'}</button></li>
//</ul>
//            `;
//            userList.appendChild(userCard);
//        });

//        userListContainer.appendChild(userList);



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
        //getUserList();
        getUserListPaging();

    });

    connection.on("ReceiveMessage", function (message) {
        console.log("Received notification: " + message);
    });

    connection.on("ReceiveMessageFromID", function (from,message) {
        console.log("Received notification: " + message);
        receiveChatMessage(from, message);
    });


    const $chatList = $('#chat-list');
    const $messageInput = $('#message-input');
    const $sendButton = $('#send-button');

    function addMessage(message, sender) {
        console.log(message + sender);
        const $messageItem = $('<li class="chat-li">');
        $messageItem.text(`${sender}: ${message}`);
        $chatList.append($messageItem);
        scrollToBottom();
    }

    function scrollToBottom() {
        var scrollingDiv = document.getElementById("chat-messages");
        scrollingDiv.scrollTop = scrollingDiv.scrollHeight;
    }

    $sendButton.click(function () {
        const message = $messageInput.val();
        if (message.trim() !== '') {
            SendMessageToRoom(message);
            $messageInput.val('');
        }
    });

    $messageInput.keypress(function (e) {
        if (e.which === 13) {
            $sendButton.click();
        }
    });
    connection.on("ReceiveMessageRoom", function (username, message) {
        addMessage(message, username);
    });

    function SendMessageToRoom(message) {
        connection.invoke("SendMessageToRoom", parseInt(userId), message).catch(function (err) {
            console.error(err.toString());
        });

    }
});
