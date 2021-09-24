(function ($) {
    function Nofication() {
        var $this = this, form; var $modal = $('#myModal');
        function initilizeForm() {

            loadSelect2();
           
            var notifyForm = new Global.FormHelper($("#frm-notification"),
                { updateTargetId: "validation-summary" },
                function onSucccess(result) {
                    alertify.set('notifier', 'position', 'top-right');
                    if (result.message && result.isSuccess) {
                        $("#frm-notification")[0].reset();
                        loadSelect2();
                        showMessage(result.message);

                    }
                   
                    if (result.message && result.isSuccess==false) {
                        alertify.error(result.message);
                       
                    }
                  
                }, null
            );
        }

        $("#myModal").on("hidden.bs.modal", function (e) {
            $(this).find('.modal-body').html("");
            //$(this).removeData('bs.modal');
        });


        function showMessage($msg) {
            $modal.find('.modal-body').append('<p>' + $msg + '</p>');
            $modal.modal('show');
        }

        function loadSelect2() {
            select2('teamSelect', 'teams', 'Choose Teams');
            select2('userSelect', 'players', 'Choose Players');
        }

        function select2($class, $action, $placeholder) {
            $("." + $class).select2({ // users-list
                minimumInputLength: 0,
                closeOnSelect: false,
                placeholder: $placeholder,
                templateResult: formatState,
                allowHtml: true,
                allowClear: true,
                ajax: {
                    url: '/home/' + $action,
                    //data: function (params) {
                    //    return {
                    //        q: params.term// search term
                    //    };
                    //},
                    processResults: function (data) {
                        return {
                            results: $.map(data.items, function (item) {
                                return {
                                    id: item.id,
                                    text: item.name,
                                    isImage: item.isImage,
                                    playerPic: item.image
                                };
                            })
                        };
                    },
                }
            }).on('change', function () {
                $(this).valid();
            });
        }
        function formatState(state) {
            if (!state.id || !state.isImage || state.isImage == false) {
                return state.text;
            }
            var $state = $(
                '<span><img style="height:18px;width:20px;" src="' + state.playerPic + '"class="img-flag" /> ' + state.text + '</span>'
            );
            return $state;
        };


        $this.init = function () {
            //validationSettings = {
            //    ignore: '.ignore'
            //};

            initilizeForm();
        }
    }

    $(function () {
        var self = new Nofication();
        self.init();
    })
})(jQuery)