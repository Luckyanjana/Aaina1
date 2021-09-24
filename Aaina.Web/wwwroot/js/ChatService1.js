
var connection, userArr;


var WhatsApp = function (app) {
    function Contact(pk, name, img, online, tyid) {
        this.id = contactList.length;
        this.pk = pk;
        this.tyid = tyid;
        this.name = name;
        this.img = img;
        this.online = online;
        this.messages = new Array();
        this.newmsg = 0;
        this.groups = new Array();
        contactList.push(this);
        return this;
    }

    Contact.prototype.addMessage = function (msg) {
        this.messages.push(msg);
    };

    Contact.prototype.addGroup = function (group) {
        this.groups.push(group);
    };

    appContacts = Contact;
    return appContacts;
}(WhatsApp || {});

//groups
var WhatsApp = function (app) {
    function Group(pk, name, img, tyid) {
        this.id = contactList.length;
        this.name = name;
        this.pk = pk;
        this.tyid = tyid;
        this.img = img;
        this.members = new Array();
        this.messages = new Array();
        this.newmsg = 0;

        contactList.push(this);
    }

    Group.prototype.addMember = function (contact) {
        this.members.push(contact);
    };

    Group.prototype.addMessage = function (msg) {
        this.messages.push(msg);
    };

    appGroups = Group;
    return appGroups;

}(WhatsApp || {});

//messages
var WhatsApp = function (app) {
    function Message(text, name, time, type, group) {
        this.text = text;
        this.name = name,
            this.time = time;
        this.type = type;
        this.group = group;
    }

    appMessages = Message;
    return appMessages;
}(WhatsApp || {});

//subject
/**
 * Created by oflox on 26.09.2020.
 */
var WhatsApp = function (app) {

    function Subject() {
        this.observers = [];
    };

    Subject.prototype.subscribe = function (item) {
        this.observers.push(item);
    };

    Subject.prototype.unsubscribeAll = function () {
        this.observers.length = 0;
    };

    Subject.prototype.notifyObservers = function () {
        this.observers.forEach(elem => { elem.notify(); });
    };

    app.Subject = Subject;
    return app;

}(WhatsApp || {});

//model
/**
* Created by oflox on 26.09.2020.
*/
var currentChat;
var contactList = new Array();

var WhatsApp = function ToDoModel(app) {
    var subject = new app.Subject();

    var Model = {
        start: function () {

            connection = new signalR.HubConnectionBuilder()
                .withUrl("/ChatHub?userId=" + userId)
                .build();

            connection.start().then(function () {
                connection.invoke('GetUserList', companyId, userId).then(function (users) {

                    userArr = users;
                    for (var i = 0; i < userArr.length; i++) {
                        if (userArr[i].sendDate != null) {
                            userArr[i].sendDate = new Date(userArr[i].sendDate);
                        }
                        if (userArr[i].type == 2) {
                            connection.invoke('Joinroup', `${userArr[i].id}`)
                        }
                    }

                    var usesAr = $.grep(users, function (n, i) {
                        return (n.type == 1);
                    });

                    usesAr = usesAr.sort((a, b) => (a.sendDate < b.sendDate) ? 1 : -1)

                    var grroupAr = $.grep(users, function (n, i) {
                        return (n.type == 2);
                    });

                    for (var i = 0; i < usesAr.length; i++) {
                        var e = usesAr[i];
                        var contact = new appContacts(e.id, e.name, e.profileImage, "10:20", "1");
                        var message = new appMessages(e.lastMessage, e.senderName, e.sendDate != '' ? moment(new Date(e.sendDate)).format('hh:mm') : '', e.type, false);
                        contact.addMessage(message);
                    }

                    for (var i = 0; i < grroupAr.length; i++) {
                        var e = grroupAr[i];

                        var group = new appGroups(e.id, e.name, e.profileImage, "2");

                        var usIds = e.userIds.split(',');

                        for (var j = 0; j < usIds.length; j++) {
                            if (window.CP.shouldStopExecution(1)) break;
                            var uIndex = contactList.findIndex((obj => obj.pk == usIds[j] && obj.group == undefined));
                            if (uIndex >= 0) {
                                group.addMember(contactList[uIndex]);
                                contactList[uIndex].addGroup(group);
                            }
                        } window.CP.exitedLoop(1);

                        var message = new appMessages(e.lastMessage, e.senderName, e.sendDate != '' ? moment(new Date(e.sendDate)).format('hh:mm') : '', e.type, false);
                        group.addMessage(message);
                    }

                    window.CP.exitedLoop(0);
                    subject.notifyObservers();

                });
            }).catch(function (err) {

                return console.error(err.toString());
            });


        },
        writeMessage: function () {
            var msg = new appMessages($(".input-message").val(), "", new Date().getHours() + ":" + new Date().getMinutes(), true);
            if (parseInt(currentChat.tyid) == 1) {
                connection.invoke('SendMessageToUser', userId, userName, userAvatar, `${currentChat.pk}`, currentChat.tyid, msg.text);
            } else {
                connection.invoke('SendMessageToGroup', `${currentChat.pk}`, userId, userName, userAvatar, msg.text);
            }

            WhatsApp.View.printMessage(msg);
            currentChat.addMessage(msg);
            $(".input-message").val("");
            $("#" + currentChat.id).addClass("active-contact");
            subject.notifyObservers();
        },
       
        getMessage: function (senderid, sendername, message) {
            
            var objIndex = contactList.findIndex((obj => obj.pk == parseInt(senderid) && obj.tyid == '1'));
            var msg = new appMessages(message, sendername, new Date().getHours() + ":" + new Date().getMinutes(), senderid == currentChat.pk, false);
          
            contactList[objIndex].addMessage(msg);
            contactList[objIndex].online = new Date().getHours() + ":" + new Date().getMinutes();

            if (contactList[objIndex] == currentChat) {
                WhatsApp.View.printMessage(msg);
                WhatsApp.View.printContact(contactList[objIndex]);
            } else {
                contactList[objIndex].newmsg++;
                WhatsApp.View.printContact(contactList[objIndex]);
            }           
        },

        getGroupMessage: function (receiverid, sendername, message) {
            
            var objIndex = contactList.findIndex((obj => obj.pk == parseInt(receiverid) && obj.tyid == '2'));
            var msg = new appMessages(message, sendername, new Date().getHours() + ":" + new Date().getMinutes(), receiverid == currentChat.pk, true);
           
            contactList[objIndex].addMessage(msg);
            contactList[objIndex].online = new Date().getHours() + ":" + new Date().getMinutes();

            if (contactList[objIndex] == currentChat) {
                WhatsApp.View.printMessage(msg);
                WhatsApp.View.printContact(contactList[objIndex]);
            } else {
                contactList[objIndex].newmsg++;
                WhatsApp.View.printContact(contactList[objIndex]);
            }
        },
        register: function (...args) {
            subject.unsubscribeAll();
            args.forEach(elem => {
                subject.subscribe(elem);
            });
        }
    };

    app.Model = Model;
    return app;

}(WhatsApp || {});

//view
/**
 * Created by oflox on 02.01.2017.
 */
var first = true;

var WhatsApp = function ToDoView(app) {
    var view = {
        printContact: function (c) {

            $("#" + c.id).remove();
            var lastmsg = c.messages[c.messages.length - 1];
            if (lastmsg != undefined) {
                if (c.newmsg == 0) {
                    var html = $("<div class='contact' id='" + c.id + "'><img src='" + c.img + "' alt='profilpicture'><div class='contact-preview'><div class='contact-text'><h1 class='font-name'>" + c.name + "</h1><p class='font-preview'>" + lastmsg.text + "</p></div></div><div class='contact-time'><p>" + lastmsg.time + "</p></div></div>");
                } else {
                    var html = $("<div class='contact new-message-contact' id='" + c.id + "'><img src='" + c.img + "' alt='profilpicture'><div class='contact-preview'><div class='contact-text'><h1 class='font-name'>" + c.name + "</h1><p class='font-preview'>" + lastmsg.text + "</p></div></div><div class='contact-time'><p>" + lastmsg.time + "</p><div class='new-message' id='nm" + c.id + "'><p>" + c.newmsg + "</p></div></div></div>");
                }
            } else {
                var html = $("<div class='contact new-message-contact' id='" + c.id + "'><img src='" + c.img + "' alt='profilpicture'><div class='contact-preview'><div class='contact-text'><h1 class='font-name'>" + c.name + "</h1><p class='font-preview'>no message</p></div></div><div class='contact-time'><p>00:00</p><div class='new-message' id='nm" + c.id + "'><p> </p></div></div></div>");
            }

            var that = c;
            $(".contact-list").prepend(html);
            WhatsApp.Ctrl.addClick(html, that);
        },
        printChat: function (cg) {

            WhatsApp.View.closeContactInformation();
            $(".chat-head img").attr("src", cg.img);
            $(".chat-name h1").text(cg.name);
            if (cg.members == undefined) {
                $(".chat-name p").text(userName + cg.online);
                // Nachrichten konfigurieren
                $(".chat-bubble").remove();
                for (var i = 0; i < cg.messages.length; i++) {
                    if (window.CP.shouldStopExecution(4)) break;
                    WhatsApp.View.printMessage(cg.messages[i]);
                } window.CP.exitedLoop(4);
                currentChat = cg;
            } else {
                var listMembers = "";
                for (var i = 0; i < cg.members.length; i++) {
                    if (window.CP.shouldStopExecution(5)) break;
                    listMembers += cg.members[i].name;
                    if (i < cg.members.length - 1) {
                        listMembers += ", ";
                    }
                } window.CP.exitedLoop(5);
                $(".chat-name p").text(listMembers);
                // Nachrichten konfigurieren
                $(".chat-bubble").remove();
                for (var i = 0; i < cg.messages.length; i++) {
                    if (window.CP.shouldStopExecution(6)) break;
                    WhatsApp.View.printMessage(cg.messages[i]);
                } window.CP.exitedLoop(6);
                currentChat = cg;
            }
        },
        printMessage: function (gc) {

            if (gc.group) {
                if (gc.type) {
                    $(".chat").append("<div class='chat-bubble me'><div class='my-mouth'></div><div class='content'>" + gc.text + "</div><div class='time'>" + gc.time + "</div></div>");
                } else {
                    $(".chat").append("<div class='chat-bubble you'><div class='your-mouth'></div><h4>" + gc.name + "</h4><div class='content'>" + gc.text + "</div><div class='time'>" + gc.time + "</div></div>");
                }
            } else {
                if (gc.type) {
                    $(".chat").append("<div class='chat-bubble me'><div class='my-mouth'></div><div class='content'>" + gc.text + "</div><div class='time'>" + gc.time + "</div></div>");
                } else {
                    $(".chat").append("<div class='chat-bubble you'><div class='your-mouth'></div><div class='content'>" + gc.text + "</div><div class='time'>" + gc.time + "</div></div>");
                }
            }
            var $messageDiv = $('.wrap-chat .chat');
            $messageDiv.scrollTop($messageDiv[0].scrollHeight);
        },
        showContactInformation: function () {
            $(".chat-head i").hide();
            $(".information").css("display", "flex");
            $("#close-contact-information").show();
            if (currentChat.members == undefined) {
                $(".information").append("<img src='" + currentChat.img + "'><div><h1>Name:</h1><p>" + currentChat.name + "</p></div><div id='listGroups'><h1>Gemeinsame Gruppen:</h1></div>");
                for (var i = 0; i < currentChat.groups.length; i++) {
                    if (window.CP.shouldStopExecution(7)) break;
                    html = $("<div class='listGroups'><img src='" + currentChat.groups[i].img + "'><p>" + currentChat.groups[i].name + "</p></div>");
                    $("#listGroups").append(html);
                    $(html).click(function (e) {
                        for (var i = 0; i < contactList.length; i++) {
                            if (window.CP.shouldStopExecution(8)) break;
                            if ($(currentChat).find("p").text() == contactList[i].name) {
                                $(".active-contact").removeClass("active-contact");
                                $("#" + contactList[i].id).addClass("active-contact");

                                WhatsApp.Groups.printChat(contactList[i]);
                            }
                        } window.CP.exitedLoop(8);
                    });
                } window.CP.exitedLoop(7);
            } else {
                $(".information").append("<img src='" + currentChat.img + "'><div><h1>Name:</h1><p>" + currentChat.name + "</p></div><div id='listGroups'><h1>Mitglieder:</h1></div>");
                for (var i = 0; i < currentChat.members.length; i++) {
                    if (window.CP.shouldStopExecution(9)) break;
                    html = $("<div class='listGroups'><img src='" + currentChat.members[i].img + "'><p>" + currentChat.members[i].name + "</p></div>");
                    $("#listGroups").append(html);
                    $(html).click(function (e) {

                        for (var i = 0; i < contactList.length; i++) {
                            if (window.CP.shouldStopExecution(10)) break;
                            if ($(currentChat).find("p").text() == contactList[i].name) {
                                $(".active-contact").removeClass("active-contact");
                                $("#" + contactList[i].id).addClass("active-contact");
                                WhatsApp.Contacts.printChat(contactList[i]);
                            }
                        } window.CP.exitedLoop(10);
                    });
                } window.CP.exitedLoop(9);
            }
        },
        closeContactInformation: function () {
            $(".chat-head i").show();
            $("#close-contact-information").hide();
            $(".information >").remove();
            $(".information").hide();
        },

        //Observer-Methode
        notify: function () {
            if (first) {
                first = false;
                for (var i = 0; i < contactList.length; i++) {
                    if (window.CP.shouldStopExecution(11)) break;
                    WhatsApp.View.printContact(contactList[i]);
                    currentChat = contactList[i];
                } window.CP.exitedLoop(11);
                first = false;
            } else {
                WhatsApp.View.printContact(currentChat);
            }
        }
    };


    app.View = view;
    return app;

}(WhatsApp);

//controller
/**
 * Created by oflox on 26.09.2020.
 */
var start = true;

var WhatsApp = function ToDoCtrl(app) {

    $(document).ready(function () {


        app.Model.start();
    });

    var Ctrl = {
        addClick: function (html, that) {
            $(html).click(function (e) {
                that.messages = [];

                connection.invoke('GetChatHistory', `${userId}`, `${that.pk}`, that.tyid).then(function (response) {
                    for (var i = 0; i < response.length; i++) {
                        var mess = response[i];
                        var message = new appMessages(mess.message, mess.senderName, moment(new Date(mess.sendDate)).format('hh:mm'), mess.senderId == userId, that.tyid == '2');
                        that.addMessage(message);
                    }

                    WhatsApp.View.printChat(that);
                });

                $(".active-contact").removeClass("active-contact");
                $(this).addClass("active-contact");
                $(this).removeClass("new-message-contact");
                $("#nm" + that.id).remove();
                that.newmsg = 0;
            });
        },
        notify: function () {
            if (start) {

                $(".input-message").keyup(function (ev) {
                    if (ev.which == 13 || ev.keyCode == 13) {
                        app.Model.writeMessage();
                    }
                });

                $(".input-search").keyup(function (ev) {
                    var filter, a, txtValue;
                    filter = $(this).val().toUpperCase();
                    $.each($(".contact-list .contact"), function () {
                        a = $(this).find('.font-name').html();
                        txtValue = a;
                        if (txtValue.toUpperCase().indexOf(filter) > -1) {
                            $(this).css('display', '');
                        } else {
                            $(this).css('display', 'none');
                        }
                    });
                });

                $("#show-contact-information").on("click", function () {
                    WhatsApp.View.showContactInformation();
                });

                $("#close-contact-information").on("click", function () {
                    WhatsApp.View.closeContactInformation();
                });


                connection.on("ReceiveMessageToUser", (senderid, sendername, message, receivertype,
                    senderprofile, receiveDateTime) => {                    
                    app.Model.getMessage(senderid, sendername, message);
                });

                connection.on("ReceiveMessageToGroup", (senderid, sendername, senderprofile, receiverId, message, receiveDateTime) => {
                    
                    app.Model.getGroupMessage(receiverId, sendername, message, '2');
                });

                start = false;
            }
        }
    };

    app.Ctrl = Ctrl;
    return app;

}(WhatsApp);

WhatsApp.Model.register(WhatsApp.View, WhatsApp.Ctrl);