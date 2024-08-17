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
    let dialog = document.getElementById("FormDialog");
    dialog.removeAttribute("open");
}

