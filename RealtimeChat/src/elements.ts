import { IChatSessionDTO, IMessageDTO } from "./dto/DTO";

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
        let img = document.createElement("img");
        img.src = "/user.jpg";
        avatar.appendChild(img);
        let userName = document.createElement("span");
        userName.innerText = session.name;
        let userLastMessage = document.createElement("span");
        userLastMessage.innerText = `Owner: ${
            session.users.filter((x) => x.id == session.ownerId)[0]
        }`;
        let idInput = document.createElement("input");
        idInput.type = "hidden";
        idInput.value = session.id.toString();

        li.appendChild(avatar);
        li.appendChild(userName);
        li.appendChild(userLastMessage);
        li.appendChild(idInput);
        return li;
    }
    div(): HTMLDivElement {
        return document.createElement("div");
    }
}
export let elements = new Elements();
