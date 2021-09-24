using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aaina.Common
{
    public enum MessageType
    {
        Warning,
        Success,
        Danger,
        Info
    }

    public enum ModalSize
    {
        Small,
        Large,
        Medium,
        XLarge
    }

    public enum UserType
    {
        SuperAdmin = 1,
        Admin = 2,
        User = 3
    }

    public enum PlayersType
    {
        Manpower = 1,
        Student = 2,
        Vendor = 3
    }

    public enum LookType
    {
        Game = 1,
        Team = 2,
        User = 3
    }

    public enum LookCalculation
    {
        [Description("Weighted Average")]
        WeightedAverage = 1,
        [Description("Last Update")]
        LastUpdate = 2,
        [Description("Selected Date")]
        ForToday = 3
    }



    public enum ScheduleType
    {
        [Description("One Time")]
        OneTime = 1,
        [Description("Recurring")]
        Recurring = 2
    }

    public enum GameType
    {
        [Description("Individual")]
        Individual = 1,
        [Description("Game Level")]
        GameLevel = 2
    }
    public enum ScheduleFrequency
    {
        [Description("Daily")]
        Daily = 1,
        [Description("Weekly")]
        Weekly = 2,
        [Description("Monthly")]
        Monthly = 3
    }
    public enum ScheduleDailyFrequency
    {
        [Description("Once")]
        Once = 1,
        [Description("Every")]
        Every = 2
    }

    public enum ScheduleMonthlyOccurrence
    {
        [Description("On the exact date")]
        Days = 1,
        [Description("On the exact weekday")]
        Week = 2,
        //[Description("Months")]
        //Months = 3,
    }
    public enum EmotionsFor
    {
        [Description("Game")]
        Game = 1,
        [Description("Team")]
        Team = 2,
        [Description("Player")]
        Player = 3,
    }

    public enum EmotionsFrom
    {

        [Description("Team")]
        Team = 1,
        [Description("Player")]
        Player = 2,
    }



    public enum ScheduleTimeUnit
    {
        [Description("Hours")]
        Hours = 1,
        [Description("Minutes")]
        Minutes = 2,
        //[Description("Months")]
        //Months = 3,
    }

    public enum SessionType
    {
        [Description("Football")]
        Football = 1,
        [Description("Meeting")]
        Meeting = 2,
        [Description("F-Cube")]
        FCube = 3
    }

    public enum ReportType
    {
        [Description("Football")]
        Football = 1,
        [Description("Non-Football")]
        NonFootball = 2
    }

    public enum PollType
    {
        [Description("Yes No")]
        YesNo = 1,
        [Description("Option")]
        Option = 2,
        [Description("Input")]
        Input = 3
    }

    public enum AccountAbilityType
    {
        [Description("Confirmed")]
        Confirmed = 1,
        [Description("Approved")]
        Approved = 2,
        [Description("Creator")]
        Creator = 3,
        [Description("Observer")]
        Observer = 4,
        [Description("Verifier")]
        Verifier = 5
    }

    public enum SessionMode
    {
        [Description("Physical")]
        Physical = 1,
        [Description("Video")]
        Video = 2,
        [Description("Call")]
        Call = 3,
        [Description("Chat")]
        Chat = 4
    }

    public enum ParticipantType
    {
        [Description("Decision Maker")]
        DecisionMaker = 1,
        [Description("Coordinator")]
        Coordinator = 2,
        [Description("Participant")]
        Participant = 3
    }


    public enum NotificationsType
    {
        [Description("Text Message")]
        TextMessage = 1,
        [Description("Email")]
        Email = 2,
        [Description("Notification")]
        Notification = 3
    }

    public enum StatusModeType
    {
        [Description("Message")]
        Message = 1,
        [Description("Email")]
        Email = 2,
        [Description("Call")]
        Call = 3
    }

    public enum NotificationsUnit
    {
        [Description("Minutes")]
        Minutes = 1,
        [Description("Hours")]
        Hours = 2,
        [Description("Days")]
        Days = 3,
        [Description("Weeks")]
        Weeks = 4
    }

    public enum ConfirmStatus
    {
        [Description("Pending")]
        Pending = 1,
        [Description("Confirmed")]
        Confirmed = 2,
        [Description("Rejected")]
        Rejected = 3
    }

    public enum UnitType
    {
        [Description("kgs")]
        KGS = 1,
        [Description("Feet")]
        Feet = 2,
        [Description("cm")]
        CM = 3,
        [Description("Inch")]
        Inch = 4,
        [Description("min")]
        Minutes = 5,

        [Description("sec")]
        Seconds = 6,
    }

    public enum PlayType
    {
        Play = 1,
        Feedback = 2
    }

    public enum PriorityType
    {
        Urgent = 1,
        High = 2,
        Medium = 3,
        Low = 4,
        Backlog = 5
    }

    public enum StatusType
    {
        Completed = 1,
        Delegate = 2,
        Active = 3,
        Inactive = 4,
        Delay = 5
    }

    public enum FeedbackType
    {
        Advice = 1,
        Approval = 2,
        Issue = 3
    }

    public enum PreSessionStatusType
    {
        Accept = 1,
        Reject = 2,
        ReSchedule = 3
    }

    public enum NotificationType
    {
        [Description("Notification")]
        Notification = 1,
        [Description("Reminder")]
        Reminder = 2,
        [Description("Look Notifiction")]
        LookNotifiction = 3,
        [Description("Session Notifiction")]
        SessionNotifiction = 4,
        [Description("Status Notifiction")]
        StatusNotifiction = 5,
        [Description("Report Notifiction")]
        ReportNotifiction = 6,
        [Description("Poll Notifiction")]
        PollNotifiction = 7,
    }

    public enum OptionType
    {
        Text = 1,
        Integer = 2,
        Date = 3,
        Time = 4,
        Checkbox = 5,
        Emotion = 6,
        Dropdown = 7,
        RadioButton = 8
        
    }

}
