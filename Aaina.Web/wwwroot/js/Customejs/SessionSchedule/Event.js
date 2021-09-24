(function ($) {

    function Index() {
        var $this = this; $activeList = 'limonth'; activeLink = 'limonth'; $activeView = 2; $calendar = null; $selectedDate = new Date();
        function InitializeCalendar() {
            try {
                $('#txt_date').datepicker({
                    keyboardNavigation: false,
                    forceParse: false,
                    toggleActive: false,
                    autoclose: true,
                    format: 'dd/mm/yyyy'
                });
            } catch (ex) {
                
            }

            var Calendar = FullCalendar.Calendar;
            var calendarEl = document.getElementById('calendar');

            // initialize the external events
            // -----------------------------------------------------------------
            

            $calendar = new Calendar(calendarEl, {
                headerToolbar: {
                    center: 'dayGridMonth,timeGridWeek,timeGridDay,listMonth',
                    left: 'title',
                    right: 'prev,next today'
                },
                aspectRatio: 1.8,                
                defaultDate: new Date(),
                navLinks: true, // can click day/week names to navigate views                               
                eventLimit: true,                                
                dayMaxEvents: 4,                                                                
                displayEventEnd: true,
                eventRender: function (event, element) {

                    $(element).tooltip({ title: event.title });
                },               
                events: [],
                eventClick: function (calEvent, jsEvent, view) {
                    
                    $selectedDate = calEvent.event.start;
                   var $selectedEndDate = calEvent.event.end;
                    var selecteddateValue = null;
                    var selectedEnddateValue = null;
                    if ($selectedDate == undefined || $selectedDate == '') { $selectedDate = null }
                    else {
                        selecteddateValue = moment(new Date($selectedDate)).format('MM/DD/YYYY HH:mm');
                        selectedEnddateValue = moment(new Date($selectedEndDate)).format('MM/DD/YYYY HH:mm')
                    }
                    var id = calEvent.event.id;
                    if (parseInt(id)>0) {
                        $('.dynamicLinkEdit').remove();   
                        var url = $("#api_agenda_url").val() +"/" + id + "?start=" + encodeURIComponent(selecteddateValue) + "&end=" + encodeURIComponent(selectedEnddateValue);
                        $("body").append('<a class="dynamicLinkEdit" id="link" title="" data-toggle="modal" data-target="#modal-add-edit-agenda_dynamicLink" data-backdrop="static"  href="' + url + '"> &nbsp</a>')
                        $('#link')[0].click();
                    }

                },
            });

            $calendar.render();
            $calendar.gotoDate($selectedDate);

            $(document).off("click", ".fc-timeGridDay-button").on("click", ".fc-timeGridDay-button", function (event) {
                refreshCalendar();
            });

            $(document).off("click", ".fc-timeGridWeek-button").on("click", ".fc-timeGridWeek-button", function (event) {
                refreshCalendar();
            });

            $(document).off("click", ".fc-dayGridMonth-button").on("click", ".fc-dayGridMonth-button", function (event) {
                refreshCalendar();
            });

            $(document).off("click", ".fc-prev-button").on("click", ".fc-prev-button", function (event) {
                refreshCalendar();
            });

            $(document).off("click", ".fc-next-button").on("click", ".fc-next-button", function (event) {
                refreshCalendar();
            });

            $(document).off("click", ".fc-today-button").on("click", ".fc-today-button", function (event) {
                refreshCalendar();
            });

            $("#txt_date").change(function () {
                var date = $(this).val();
                
                $calendar.gotoDate(new Date(moment(date, 'DD/MM/yyyy').format("MM/DD/YYYY")));
                refreshCalendar();
            });

            $("#game_id").on('change', function () {
                refreshCalendar();
            });

            $('#IsReport').change(function () {
                refreshCalendar();
            });

            $('#IsSession').change(function () {
                refreshCalendar();
            });

            // fc-prev-button
        }





        function bindEvents() {
            var date = new Date()

            var ss = $calendar.getCurrentData().dateProfile.activeRange;
            var vie = $calendar.getCurrentData().currentViewType;
            var isReort = $("#IsReport").is(":checked");
            var isSession = $("#IsSession").is(":checked");

            $.get(`${$("#api_url").val()}?gameId=${$("#game_id").val()}&start=${moment(ss.start).format("MM/DD/YYYY")}&end=${moment(ss.end).format("MM/DD/YYYY")}&isSession=${isSession}&isReport=${isReort}`, function (result) {
                
                $calendar.addEventSource(result);
            })

        }


        function refreshCalendar() {
            $selectedDate = $calendar.getDate();

            var source = $calendar.getEventSources();
            source[0].remove();
            source[0].refetch();
            bindEvents();
        }


        this.init = function () {

            InitializeCalendar();
            $(".fc-dayGridMonth-button").trigger("click");
            // $('#limonth').addClass("active");

        }
    }


    $(function () {

        var self = new Index();
        self.init();

    })

})(jQuery)