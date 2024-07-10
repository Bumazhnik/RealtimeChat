import { IChatSessionDTO, IMessageDTO } from "./dto/DTO";
import { myUser } from "./myUser";
import { getJson } from "./requests";
import { globalState } from "./state";

class Elements {
    sessionList = document.getElementsByClassName("session-list")[0];
    chatContent = document.getElementsByClassName("chat-content")[0];
    hUserNames = <HTMLDivElement>(
        document.getElementsByClassName("h-user-names")[0]
    );
    liList = this.sessionList.querySelectorAll("li");
    sendButton = <HTMLButtonElement>document.getElementById("send");
    messageField = <HTMLInputElement>document.getElementById("message-field");

    currentSessionElement: HTMLLIElement;

    createMessageElement(
        message: IMessageDTO,
        isSelf: boolean
    ): HTMLDivElement {
        let msgDiv = this.div();
        msgDiv.classList.add("message");
        if (isSelf) msgDiv.classList.add("self");
        let textDiv = this.div();
        textDiv.classList.add("text");

        msgDiv.appendChild(textDiv);

        let nameText = document.createElement("h6");
        nameText.innerText = message.userName;
        let text = document.createElement("span");
        text.innerText = message.text;
        let date = document.createElement("span");
        date.classList.add("date");
        let d = new Date(message.date);
        date.innerText =
            ("0" + d.getHours()).slice(-2) +
            ":" +
            ("0" + d.getMinutes()).slice(-2);
        textDiv.appendChild(nameText);
        textDiv.appendChild(text);
        textDiv.appendChild(date);
        return msgDiv;
    }
    createSessionElement(session: IChatSessionDTO): HTMLLIElement {
        let li = document.createElement("li");
        let avatar = this.div();
        avatar.classList.add("avatar");
        let img = document.createElement("img");
        img.src = "/user.jpg";
        avatar.appendChild(img);
        let userName = document.createElement("span");
        userName.classList.add("user-name");
        userName.innerText = session.name;
        let userLastMessage = document.createElement("span");
        userLastMessage.classList.add("user-last-message");
        userLastMessage.innerText = `Owner: ${
            session.users.filter((x) => x.id == session.ownerId)[0].name
        }`;
        let idInput = document.createElement("input");
        idInput.type = "hidden";
        idInput.value = session.id.toString();

        li.appendChild(avatar);
        li.appendChild(userName);
        li.appendChild(userLastMessage);
        li.appendChild(idInput);
        li.addEventListener("click", (x) => this.onSessionClick(li));
        return li;
    }
    onSessionClick(sessionLi: HTMLLIElement) {
        if (
            this.currentSessionElement &&
            this.currentSessionElement.classList.contains("current")
        )
            this.currentSessionElement.classList.remove("current");
        this.currentSessionElement = sessionLi;
        this.currentSessionElement.classList.add("current");
        globalState.currentSessionId = +sessionLi.querySelector("input").value;
        this.loadMessages(globalState.currentSessionId);
        this.loadSession(globalState.currentSessionId);
    }
    async loadSession(id: number) {
        console.log(`session number ${id} loading...`);
        let sessions: IChatSessionDTO = await getJson(
            `/chat/sessionInfo/${id}`
        );

        console.log(sessions);
        this.hUserNames.innerText = sessions.users
            .map((x) => x.name)
            .join(", ");
    }
    async loadMessages(id: number) {
        console.log(`messages of session number ${id} loading...`);
        let messages: IMessageDTO[] = await getJson(`/chat/messages/${id}`);

        console.log(messages);
        this.chatContent.innerHTML = "";
        for (let m of messages) {
            this.chatContent.appendChild(
                this.createMessageElement(m, myUser.name == m.userName)
            );
        }
    }
    div(): HTMLDivElement {
        return document.createElement("div");
    }
}
export let elements = new Elements();
