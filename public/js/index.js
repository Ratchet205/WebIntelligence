let chatCounter = 0;
const IDbegin = "ChatID-";


function createChat() {
    let ChatCount = chatCounter++;
    let chat = document.createElement("button");

    chat.setAttribute("id", IDbegin+ChatCount);
    chat.setAttribute("class", "chatButton");
    chat.textContent = "Chat "+ChatCount;
    chat.setAttribute("style", 'z-index:99, position:absolute');


    let navbar = document.getElementById("container-left");

    navbar.appendChild(chat);
}

function debugChatLoop() {
    let chat_nav = document.getElementById('chat-list-ul');
    for(let i = 0; i < 50; i++) {
        chat_nav.innerHTML += '<li class="chat-item"><span class="chat-name">'+`Chat${i}`+'</span><span class="chat-icons"><i class="icon edit-icon">✎</i><i class="icon delete-icon">✘</i></span></li>';
    }
}

function debugLogin() {
    let dialog = document.getElementById("logindialog");
    dialog.removeAttribute("open");
}
function openRegistration() {
    let login = document.getElementById("logindialog");
    login.removeAttribute("open");
    let registration = document.getElementById("registrationdialog");
    registration.setAttribute("open", "")
}
function openLogin() {
    let registration = document.getElementById("registrationdialog");
    registration.removeAttribute("open");
    let login = document.getElementById("logindialog");
    login.setAttribute("open", "")
}

var auth2;

function initClient() {
    gapi.load('auth2', function() {
        auth2 = gapi.auth2.init({
            client_id: '334094857641-ef4sau3ua1clm53gm6jcashbtkl3mpuf.apps.googleusercontent.com',
            cookiepolicy: 'single_host_origin',
        });
    });
}

function signIn() {
        auth2.signIn().then(function(googleUser) {
            onSignIn(googleUser);
        }).catch(function(error) {
            console.error('Sign-in error', error);
        });
}

function onSignIn(googleUser) {
    var profile = googleUser.getBasicProfile();
    console.log('ID: ' + profile.getId());
    console.log('Name: ' + profile.getName());
    console.log('Image URL: ' + profile.getImageUrl());
    console.log('Email: ' + profile.getEmail());
}

document.addEventListener('DOMContentLoaded', function() {
    initClient();
    document.getElementById('customGoogleSignIn').addEventListener('click', signIn);
});