import './AppController.scss';

import {Interface} from "./Utility/Interface";
import {NetworkController} from "./Networking/NetworkController";
import {Database} from "./Database";
import {ControlPanel} from "./Interface/ControlPanel";
import {Chatbox} from "./Interface/Chatbox";
import {MainTopBarSection} from "./Interface/MainTopBarSection";
import {Section} from "./Interface/Section/Section";
import {Utility} from "./Utility/Utility";
import {MessageReader} from "./Utility/Message/MessageReader";
import {Controllers} from "./Networking/MessageDefinitions/ReceiveMessageDefinitions";
import {CommentInfo} from "./Interface/Section/CommentInfo";

export class AppController {
    mainDiv: HTMLDivElement;
    networkController: NetworkController;
    websiteDiv: HTMLDivElement;
    aheadWebsiteDiv: HTMLDivElement;
    behindWebsiteDiv: HTMLDivElement;
    database: Database;
    controlPanel: ControlPanel;
    logicTimer: any;
    chatbox: Chatbox;
    mainSection: MainTopBarSection;
    sections: Section[] = [];
    showingSection: Section = null;
    body: HTMLBodyElement;
    reconnectingDiv: HTMLDivElement;

    constructor() {
        this.mainDiv = Interface.Create({
            type: 'div', className: 'ApplicationDiv', elements: [
                this.behindWebsiteDiv = Interface.Create({type: 'div', className: 'behindWebsite'}),
                this.websiteDiv = Interface.Create({type: 'div', className: 'Website'}),
                this.aheadWebsiteDiv = Interface.Create({type: 'div', className: 'aheadWebsite'})
            ]
        });
        this.body = document.body as HTMLBodyElement;

        this.logicTimer = setInterval(this.logic, 16);

        this.setUp();
    }

    setUp() {
        Utility.ClearElements(this.behindWebsiteDiv);
        Utility.ClearElements(this.websiteDiv);
        Utility.ClearElements(this.aheadWebsiteDiv);

        this.networkController = new NetworkController(this);
        this.database = new Database(this);
        this.controlPanel = new ControlPanel(this);
        //Put the chat on the right side
        this.chatbox = new Chatbox(this);

        //Now add the main parts of the site
        this.mainSection = new MainTopBarSection(this);


        this.networkController.initialize();
    }

    logic = () => {
        this.mainSection.logic();
        for (let section of this.sections) {
            section.logic();
        }
    };

    prepareReset() {
        Utility.ClearElements(this.behindWebsiteDiv);
        Utility.ClearElements(this.websiteDiv);
        Utility.ClearElements(this.aheadWebsiteDiv);
        this.behindWebsiteDiv.hidden = true;
        this.websiteDiv.hidden = true;
        this.aheadWebsiteDiv.hidden = true;
        if (this.reconnectingDiv != null) {
            this.reconnectingDiv.remove();
            this.reconnectingDiv = null;
        }
        this.reconnectingDiv = Interface.Create({
            type: 'div',
            className: 'ReconnectingDiv',
            text: 'Unable to connect...'
        });
        this.body.appendChild(this.reconnectingDiv);

        this.reset();
    }

    reset() {
        this.setUp();
    }

    connectedToServer() {
        this.behindWebsiteDiv.hidden = false;
        this.websiteDiv.hidden = false;
        this.aheadWebsiteDiv.hidden = false;
        if (this.reconnectingDiv != null) {
            this.reconnectingDiv.remove();
            this.reconnectingDiv = null;
        }
        //Try auto-login
        let localStorage: Storage = window.localStorage;
        if (localStorage.getItem('Name') != null && localStorage.getItem('Password')) {
            this.controlPanel.loginAlreadyEncrypted(localStorage.getItem('Name'), localStorage.getItem('Password'));
        }
    }

    showView(sectionId: string) {
        if (this.showingSection != null) {
            this.showingSection.getDiv().style.opacity = "0.0";
            this.showingSection.getDiv().remove();
            this.showingSection.hide();
        }
        this.showingSection = this.getSection(sectionId);
        if (this.showingSection != null) {
            this.showingSection.getDiv().style.opacity = "1.0";
            this.websiteDiv.appendChild(this.showingSection.getDiv());
            this.showingSection.show();
        }
    }

    goTo(sectionId: string, threadID: string) {
        let section = this.getSection(sectionId);
        if (section != null) {
            this.mainSection.sectionClick(this.sections.indexOf(section));
            section.showThread(threadID);
        }
    }

    getSection(sectionId: String): Section {
        for (let section of this.sections) {
            if (section.getId() == sectionId) {
                return section;
            }
        }
        return null;
    }

    Message(message: MessageReader) {
        let controller = message.getUint8();
        let messageID = message.getUint8();
        switch (controller) {
            case Controllers.Chat.ID: {
                switch (messageID) {
                    case Controllers.Chat.Messages.NewMessage: {
                        this.chatbox.gotNewMessage(message);
                    }
                        break;
                    case Controllers.Chat.Messages.AllOnlineList: {
                        this.chatbox.gotAllOnlineList(message);
                    }
                        break;
                    case Controllers.Chat.Messages.AllMessages: {
                        this.chatbox.gotAllMessages(message);
                    }
                        break;
                    case Controllers.Chat.Messages.DisplayNameUpdate: {
                        this.chatbox.gotDisplayNameUpdate(message);
                    }
                        break;
                    case Controllers.Chat.Messages.OnlineListAddUser: {
                        this.chatbox.gotOnlineListAddUser(message);
                    }
                        break;
                    case Controllers.Chat.Messages.OnlineListLoggedInUser: {
                        this.chatbox.gotOnlineListLoggedInUser(message);
                    }
                        break;
                    case Controllers.Chat.Messages.OnlineListLoggedOutUser: {
                        this.chatbox.gotOnlineListLoggedOutUser(message);
                    }
                        break;
                    case Controllers.Chat.Messages.OnlineListRemoveUser: {
                        this.chatbox.gotOnlineListRemoveUser(message);
                    }
                        break;
                }
            }
                break;
            case Controllers.Profile.ID: {
                switch (messageID) {
                    case Controllers.Profile.Messages.ReturnAvatar: {
                        this.controlPanel.preferencesAvatarInput.value = message.getString();
                    }
                        break;
                    case Controllers.Profile.Messages.ReturnDisplayName: {
                        this.controlPanel.preferencesDisplayNameInput.value = message.getString();
                    }
                        break;
                    case Controllers.Profile.Messages.LoggedIn: {
                        this.database.processEvent('Logged In', message);
                        this.controlPanel.processEvent('Logged In');
                        this.chatbox.processEvent('Logged In');
                        for (let section of this.sections) section.processEvent('Logged In');
                    }
                        break;
                    case Controllers.Profile.Messages.LoggedOut: {
                        this.database.processEvent('Logged Out', message);
                        this.controlPanel.processEvent('Logged Out');
                        this.chatbox.processEvent('Logged Out');
                        for (let section of this.sections) section.processEvent('Logged Out');
                    }
                        break;
                    case Controllers.Profile.Messages.LoginFailed: {
                        this.controlPanel.processEvent('Login Failed');
                    }
                        break;
                    case Controllers.Profile.Messages.RegisterFailed: {
                        this.controlPanel.processEvent('Register Failed');
                    }
                        break;
                }
            }
                break;
            case Controllers.Section.ID: {
                switch (messageID) {
                    case Controllers.Section.Messages.AllSectionViewers: {
                        let sectionId = message.getString();
                        let section = this.sections.find(value => value.sectionId == sectionId);
                        if (section) {
                            section.threadController.sectionViewingUsers = [];
                            let count = message.getUint32();
                            for (let index = 0; index < count; index++) {
                                let connectionId = message.getString();
                                let hasAccount = message.getUint8() == 1;
                                let accountId: string | undefined = hasAccount ? message.getString() : undefined;
                                let displayName: string | undefined = hasAccount ? message.getString() : undefined;
                                section.threadController.sectionViewingUsers.push({
                                    connectionId: connectionId,
                                    accountId: accountId,
                                    displayName: displayName
                                });
                            }
                            section.threadController.updateSectionViewersDisplay();
                        }
                    }
                        break;
                    case Controllers.Section.Messages.SectionAddViewer: {
                        let sectionId = message.getString();
                        let section = this.sections.find(value => value.sectionId == sectionId);
                        let connectionId = message.getString();
                        let hasAccount = message.getUint8() == 1;
                        let accountId: string | undefined = hasAccount ? message.getString() : undefined;
                        let displayName: string | undefined = hasAccount ? message.getString() : undefined;
                        section.threadController.sectionViewingUsers.push({
                            connectionId: connectionId,
                            accountId: accountId,
                            displayName: displayName
                        });
                        section.threadController.updateSectionViewersDisplay();
                    }
                        break;
                    case Controllers.Section.Messages.SectionRemoveViewer: {
                        let sectionId = message.getString();
                        let section = this.sections.find(value => value.sectionId == sectionId);
                        let connectionId = message.getString();
                        for (let index = 0; index < section.threadController.sectionViewingUsers.length; index++) {
                            let user = section.threadController.sectionViewingUsers[index];
                            if (user.connectionId == connectionId) {
                                section.threadController.sectionViewingUsers.splice(index, 1);
                                index -= 1;
                            }
                        }
                        section.threadController.updateSectionViewersDisplay();
                    }
                        break;
                    case Controllers.Section.Messages.SectionLoggedInViewer: {
                        let sectionId = message.getString();
                        let section = this.sections.find(value => value.sectionId == sectionId);
                        let connectionId = message.getString();
                        let accountId = message.getString();
                        let displayName = message.getString();
                        for (let index = 0; index < section.threadController.sectionViewingUsers.length; index++) {
                            let user = section.threadController.sectionViewingUsers[index];
                            if (user.connectionId == connectionId) {
                                user.accountId = accountId;
                                user.displayName = displayName;
                            }
                        }
                        section.threadController.updateSectionViewersDisplay();
                    }
                        break;
                    case Controllers.Section.Messages.SectionLoggedOutViewer: {
                        let sectionId = message.getString();
                        let section = this.sections.find(value => value.sectionId == sectionId);
                        let connectionId = message.getString();
                        for (let index = 0; index < section.threadController.sectionViewingUsers.length; index++) {
                            let user = section.threadController.sectionViewingUsers[index];
                            if (user.connectionId == connectionId) {
                                user.accountId = null;
                                user.displayName = null;
                            }
                        }
                        section.threadController.updateSectionViewersDisplay();
                    }
                        break;
                    case Controllers.Section.Messages.AllSectionHeaders: {
                        let count = message.getUint32();
                        for (let index = 0; index < count; index++) {
                            let sectionId = message.getString();
                            let sectionName = message.getString();
                            let title = message.getString();
                            let theme = message.getString();
                            let background = message.getString();
                            let createdTime = message.getString();
                            let useDarkTheme = theme === 'dark';
                            let hasMatrixBackground = background === 'matrix';
                            if (!this.sections.find(value => value.sectionId == sectionId)) {
                                this.sections.push(
                                    new Section({
                                        appController: this,
                                        sectionId: sectionId,
                                        displayName: sectionName,
                                        title: title,
                                        darkTheme: useDarkTheme,
                                        hasMatrixBackground: hasMatrixBackground,
                                        createdTime: createdTime
                                    })
                                );
                            }
                        }
                        this.sections.sort((a, b) => Utility.TimeuuidToMilliseconds(a.createdTime) - (Utility.TimeuuidToMilliseconds(b.createdTime)));
                        this.mainSection.addSections();
                        //Load the first section
                        this.mainSection.sectionClick(0);
                    }
                        break;
                    case Controllers.Section.Messages.ThreadSuccessfullyCreated: {
                        // Todo: This is probably where we want to switch the user's viewing thread to the thread they just created
                    }
                        break;
                    case Controllers.Section.Messages.UpdateThreadCommentCount: {
                        let sectionId = message.getString();
                        let threadId = message.getString();
                        let commentCount = message.getUint32();
                        let section = this.sections.find(value => value.sectionId == sectionId);
                        if (section) {
                            section.threadController.updateCommentCount(threadId, commentCount);
                        }
                    }
                        break;
                    case Controllers.Section.Messages.AvatarUpdate: {
                        let accountId = message.getString();
                        let avatarUrl = message.getString();
                        // Todo: We will want to tell all sections and threads to update the accoutId's avatar
                    }
                        break;
                    case Controllers.Section.Messages.DisplayNameUpdate: {
                        let accountId = message.getString();
                        let displayName = message.getString();
                        // Todo: We will want to tell all sections and threads to update the accoutId's display name
                    }
                        break;
                    case Controllers.Section.Messages.AddThreadHeader: {
                        let sectionId = message.getString();
                        let threadId = message.getString();
                        let creatorAccountId = message.getString();
                        let title = message.getString();
                        let description = message.getString();
                        let createdTime = message.getString();
                        let updatedTime = message.getString();
                        let commentCount = message.getUint32();
                        let creatorDisplayName = message.getString();
                        let creatorAvatarUrl = message.getString();
                        let section = this.sections.find(value => value.sectionId == sectionId);
                        if (section) {
                            section.addThread(threadId,
                                creatorAccountId,
                                title,
                                description,
                                createdTime,
                                updatedTime,
                                commentCount,
                                creatorDisplayName,
                                creatorAvatarUrl);
                        }
                        if (section && section.showThreadWhenLoaded != null) {
                            section.threadController.showThread(section.showThreadWhenLoaded);
                            section.showThreadWhenLoaded = null;
                        }
                    }
                        break;
                    case Controllers.Section.Messages.UpdateThreadTitleAndDescription: {
                        let sectionId = message.getString();
                        let threadId = message.getString();
                        let title = message.getString();
                        let description = message.getString();
                        let section = this.sections.find(value => value.sectionId == sectionId);
                        if (section) {
                            section.threadController.updateThread(threadId, title, description);
                        }
                    }
                        break;
                    case Controllers.Section.Messages.RemoveThreadHeader: {
                        let sectionId = message.getString();
                        let threadId = message.getString();
                        let section = this.sections.find(value => value.sectionId == sectionId);
                        if (section) {
                            section.threadController.removeThread(threadId);
                        }
                    }
                        break;
                    case Controllers.Section.Messages.UpdateThreadCommentCountAndUpdatedTime: {
                        let sectionId = message.getString();
                        let threadId = message.getString();
                        let commentCount = message.getUint32();
                        let updatedTime = message.getString();
                        let section = this.sections.find(value => value.sectionId == sectionId);
                        if (section) {
                            section.threadController.updateCommentCount(threadId, commentCount);
                            section.threadController.updateUpdatedTime(threadId, updatedTime);
                        }
                    }
                        break;
                    case Controllers.Section.Messages.AllThreadHeaders: {
                        // Todo: Measure latency in getting thread headers, when running locally, it is noticeable,
                        // it should be instant when running locally. We'll need to profile and optimize.
                        let count = message.getUint32();
                        let section = null;
                        for (let index = 0; index < count; index++) {
                            let sectionId = message.getString();
                            let threadId = message.getString();
                            let creatorAccountId = message.getString();
                            let title = message.getString();
                            let description = message.getString();
                            let createdTime = message.getString();
                            let updatedTime = message.getString();
                            let commentCount = message.getUint32();
                            let creatorDisplayName = message.getString();
                            let creatorAvatarUrl = message.getString();
                            section = this.sections.find(value => value.sectionId == sectionId);
                            if (section) {
                                section.addThread(threadId,
                                    creatorAccountId,
                                    title,
                                    description,
                                    createdTime,
                                    updatedTime,
                                    commentCount,
                                    creatorDisplayName,
                                    creatorAvatarUrl);
                            }
                        }
                        if (section && section.showThreadWhenLoaded != null) {
                            section.threadController.showThread(section.showThreadWhenLoaded);
                            section.showThreadWhenLoaded = null;
                        }
                    }
                        break;
                }
            }
                break;
            case Controllers.Thread.ID: {
                switch (messageID) {
                    case Controllers.Thread.Messages.AllThreadViewers: {
                        let sectionId = message.getString();
                        let section = this.sections.find(value => value.sectionId == sectionId);
                        if (section) {
                            let threadId = message.getString();
                            let thread = section.threadController.getThread(threadId);
                            if (thread) {
                                thread.fullView.threadDisplay.threadViewingUsers = [];
                                let count = message.getUint32();
                                for (let index = 0; index < count; index++) {
                                    let connectionId = message.getString();
                                    let hasAccount = message.getUint8() == 1;
                                    let accountId: string | undefined = hasAccount ? message.getString() : undefined;
                                    let displayName: string | undefined = hasAccount ? message.getString() : undefined;
                                    thread.fullView.threadDisplay.threadViewingUsers.push({
                                        connectionId: connectionId,
                                        accountId: accountId,
                                        displayName: displayName
                                    });
                                }
                                thread.fullView.threadDisplay.updateThreadViewersDisplay();
                            }
                        }
                    }
                        break;
                    case Controllers.Thread.Messages.ThreadAddViewer: {
                        let sectionId = message.getString();
                        let section = this.sections.find(value => value.sectionId == sectionId);
                        let threadId = message.getString();
                        let thread = section.threadController.getThread(threadId);
                        if (thread) {
                            let connectionId = message.getString();
                            let hasAccount = message.getUint8() == 1;
                            let accountId: string | undefined = hasAccount ? message.getString() : undefined;
                            let displayName: string | undefined = hasAccount ? message.getString() : undefined;
                            thread.fullView.threadDisplay.threadViewingUsers.push({
                                connectionId: connectionId,
                                accountId: accountId,
                                displayName: displayName
                            });
                            thread.fullView.threadDisplay.updateThreadViewersDisplay();
                        }
                    }
                        break;
                    case Controllers.Thread.Messages.ThreadRemoveViewer: {
                        let sectionId = message.getString();
                        let section = this.sections.find(value => value.sectionId == sectionId);
                        let threadId = message.getString();
                        let thread = section.threadController.getThread(threadId);
                        if (thread) {
                            let connectionId = message.getString();
                            for (let index = 0; index < thread.fullView.threadDisplay.threadViewingUsers.length; index++) {
                                let user = thread.fullView.threadDisplay.threadViewingUsers[index];
                                if (user.connectionId == connectionId) {
                                    thread.fullView.threadDisplay.threadViewingUsers.splice(index, 1);
                                    index -= 1;
                                }
                            }
                            thread.fullView.threadDisplay.updateThreadViewersDisplay();
                        }
                    }
                        break;
                    case Controllers.Thread.Messages.ThreadLoggedInViewer: {
                        let sectionId = message.getString();
                        let section = this.sections.find(value => value.sectionId == sectionId);
                        let threadId = message.getString();
                        let thread = section.threadController.getThread(threadId);
                        if (thread) {
                            let connectionId = message.getString();
                            let accountId = message.getString();
                            let displayName = message.getString();
                            for (let index = 0; index < thread.fullView.threadDisplay.threadViewingUsers.length; index++) {
                                let user = thread.fullView.threadDisplay.threadViewingUsers[index];
                                if (user.connectionId == connectionId) {
                                    user.accountId = accountId;
                                    user.displayName = displayName;
                                }
                            }
                            thread.fullView.threadDisplay.updateThreadViewersDisplay();
                        }
                    }
                        break;
                    case Controllers.Thread.Messages.ThreadLoggedOutViewer: {
                        let sectionId = message.getString();
                        let section = this.sections.find(value => value.sectionId == sectionId);
                        let threadId = message.getString();
                        let thread = section.threadController.getThread(threadId);
                        if (thread) {
                            let connectionId = message.getString();
                            for (let index = 0; index < thread.fullView.threadDisplay.threadViewingUsers.length; index++) {
                                let user = thread.fullView.threadDisplay.threadViewingUsers[index];
                                if (user.connectionId == connectionId) {
                                    user.accountId = null;
                                    user.displayName = null;
                                }
                            }
                            thread.fullView.threadDisplay.updateThreadViewersDisplay();
                        }
                    }
                        break;
                    case Controllers.Thread.Messages.AllComments: {
                        let count = message.getUint32();
                        for (let index = 0; index < count; index++) {
                            let sectionId = message.getString();
                            let threadId = message.getString();
                            let commentId = message.getString();
                            let creatorAccountId = message.getString();
                            let text = message.getString();
                            let creatorDisplayName = message.getString();
                            let creatorAvatarUrl = message.getString();
                            let section = this.sections.find(value => value.sectionId == sectionId);
                            if (section) {
                                let thread = section.threadController.getThread(threadId);
                                if (thread) {
                                    if (index == 0) {
                                        thread.removeAllComments();
                                    }
                                    let newComment = new CommentInfo(thread, section.threadController, section.threadController.hasDarkTheme);
                                    newComment.setCommentIDAndCreatedTime(commentId);
                                    newComment.setCreatorAccountId(creatorAccountId);
                                    newComment.setComment(text);
                                    newComment.setCreatorDisplayName(creatorDisplayName);
                                    newComment.setCreatorAvatarUrl(creatorAvatarUrl);
                                    thread.addComment(newComment);
                                }
                            }
                        }
                    }
                        break;
                    case Controllers.Thread.Messages.CommentSuccessfullyCreated: {
                        /*
        message.AddString(sectionId.ToString());
        message.AddString(threadId.ToString());
        message.AddString(commentId.ToString());
                         */
                    }
                        break;
                    case Controllers.Thread.Messages.AddComment: {
                        let sectionId = message.getString();
                        let threadId = message.getString();
                        let commentId = message.getString();
                        let creatorAccountId = message.getString();
                        let text = message.getString();
                        let creatorDisplayName = message.getString();
                        let creatorAvatarUrl = message.getString();
                        let section = this.sections.find(value => value.sectionId == sectionId);
                        if (section) {
                            let thread = section.threadController.getThread(threadId);
                            if (thread) {
                                let newComment = new CommentInfo(thread, section.threadController, section.threadController.hasDarkTheme);
                                newComment.setCommentIDAndCreatedTime(commentId);
                                newComment.setCreatorAccountId(creatorAccountId);
                                newComment.setComment(text);
                                newComment.setCreatorDisplayName(creatorDisplayName);
                                newComment.setCreatorAvatarUrl(creatorAvatarUrl);
                                thread.addComment(newComment);
                            }
                        }
                    }
                        break;
                    case Controllers.Thread.Messages.UpdateComment: {
                        let sectionId = message.getString();
                        let threadId = message.getString();
                        let commentId = message.getString();
                        let text = message.getString();
                        let section = this.sections.find(value => value.sectionId == sectionId);
                        if (section) {
                            section.threadController.updateComment(threadId, commentId, text);
                        }
                    }
                        break;
                    case Controllers.Thread.Messages.RemoveComment: {
                        let sectionId = message.getString();
                        let threadId = message.getString();
                        let commentId = message.getString();
                        let section = this.sections.find(value => value.sectionId == sectionId);
                        if (section) {
                            section.threadController.deleteComment(threadId, commentId);
                        }
                    }
                        break;
                    case Controllers.Thread.Messages.AvatarUpdate: {
                        /*
        message.AddString(accountId.ToString());
        message.AddString(avatarUrl);
                         */
                    }
                        break;
                    case Controllers.Thread.Messages.DisplayNameUpdate: {
                        /*
        message.AddString(accountId.ToString());
        message.AddString(displayName);
                         */
                    }
                        break;
                }
            }
                break;
        }
    }

    darkTheme() {
        this.body.classList.add('DarkTheme');
        this.body.classList.remove('LightTheme');
    }

    lightTheme() {
        this.body.classList.add('LightTheme');
        this.body.classList.remove('DarkTheme');
    }

    getInterface() {
        return this.mainDiv;
    }

    getWebsiteDiv(): HTMLDivElement {
        return this.websiteDiv;
    }
}