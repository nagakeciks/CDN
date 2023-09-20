const token = sessionStorage.getItem('token');
const userId = sessionStorage.getItem('userId');

//Check if token exist
if (!token) {
    //redirect unauthorized user to the login page
    window.location.href = 'login.html';
}


const intervalId = setInterval(function () {
    isTokenValid();
}, 60000);

if (!isTokenValid(token))
{
    console.log('Token has expired or is invalid.')
    alert('Session expired. Please re-login');
    window.location.href = 'login.html';
}

const headers = new Headers({
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json',
});

function isTokenValid() {
    console.log('Validating Token');
    // Decode the JWT token without verifying the signature
    const tokenData = parseJwt(token);
    if (!tokenData) {
        // Token couldn't be parsed; it's invalid
        return false;
    }

    // Check if the token has expired
    const currentTimestamp = Math.floor(Date.now() / 1000); // Current time in seconds
    return tokenData.exp > currentTimestamp;
}

function parseJwt(token) {
    try {
        // Decode the token and extract the payload
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(atob(base64).split('').map(function (c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));

        return JSON.parse(jsonPayload);
    } catch (e) {
        // Invalid token or unable to parse it
        return null;
    }
}



//if (isValid) {
//    console.log('Token is still valid.');
//} else {
//    console.log('Token has expired or is invalid.');
//}