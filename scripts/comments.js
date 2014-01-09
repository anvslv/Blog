;(function () {
    var postId = null;
      
    var endpoint = "/comment.ashx";

    function deleteComment(commentId, element) { 
        if (confirm("Do you want to delete this comment?")) {
            $.post(endpoint, {
                mode: "delete",
                postId: postId,
                commentId: commentId
            }).success(function() { 
               $(element).fadeOut(); 
            }).fail(function() {
                alert("Something went wrong. Please try again");
            }); 
        }
    }

    function saveComment(name, email, website, content, callback) {
        if (localStorage) {
            localStorage.setItem("name", name);
            localStorage.setItem("email", email);
            localStorage.setItem("website", website);
        }

        $.post(endpoint, {
            mode: "save",
            postId: postId,
            name: name,
            email: email,
            website: website,
            content: content
        }).success(function(data) {
             
            var elemStatus = $("#status");
           
            elemStatus.innerText = "Your comment has been added";
            elemStatus.removeClass("alert-danger");
            elemStatus.addClass("alert-success-dark");

            document.getElementById("commentcontent").value = "";

           // $.ajax(data).success(function (html) {
                var comment = $.parseHTML(data);
                bindDelete(comment);

                $(comment).hide().appendTo("#comments").fadeIn();

                callback(true);
            //});

        }).fail(function(data) {
            var elemStatus = document.getElementById("status");
            addClass(elemStatus, "alert-danger");
            elemStatus.innerText = data.statusText; 

            callback(false);
        });

    }

    function bindDelete(element) { 
        $(element).on("click", function (e) {
            e.preventDefault();
            var comment = $(element).closest("[itemprop=comment]"); 
            deleteComment(comment.attr("data-id"), comment);
        });
    }

    function initialize() {
        postId = $("[itemprop=blogPost]").attr("data-id");
        var email = $("#commentemail");
        var name = $("#commentname");
        var website = $("#commenturl");
        var content = $("#commentcontent");
        var commentForm = $("#commentform");
         
        commentForm.submit(function (e) {
            e.preventDefault();
            var button = e.target;
            button.setAttribute("disabled", true);

            saveComment(name.val(), email.val(), website.val(), content.val(), function () {
                button.removeAttribute("disabled");
            });
        });

        website.keyup(function (e) {
            var w = e.target;
            if (w.value.trim().length >= 4 && w.value.indexOf("http") === -1) {
                w.value = "http://" + w.value;
            }
        });

        $(".deletecomment").each(function() {
            bindDelete($(this));
        });
          
        if (localStorage) {
            email.val(localStorage.getItem("email"));
            website.val(localStorage.getItem("website"));

            if (name.val().length === 0) {
                name.val(localStorage.getItem("name"));
            }
        }
    }

    if (document.getElementById("commentform")) {
        initialize();
    }

})();