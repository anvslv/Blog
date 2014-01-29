// https://github.com/timsayshey/Ghost-Markdown-Editor

/*  jquery, magnific popup, showdown, codemirror, shortcuts, 
    codemirror inline attach, markdown actions, underscore, path */
;(function ($, CodeMirror, Path, App, Resources) {
    "use strict";

    String.prototype.endsWith = function (suffix) {
        return this.indexOf(suffix, this.length - suffix.length) !== -1;
    };

    var converter, editor,
        postId, isNew, txtTitle,
        txtMessage, chkPublish, chkComments,
        btnNew, btnEdit, btnDelete,
        btnSave, btnCancel, isEdited,
        btnDrafts;

    var editPost = function() {
            $.post('/post.ashx?mode=getMarkdown', {
                id: postId
            }).success(function(data) {
                initEditor(data);
                showTitleAndCategoriesForEditing();
                btnDrafts.addClass("pure-menu-disabled");
                btnNew.addClass("pure-menu-disabled");
                btnEdit.addClass("pure-menu-disabled");
                btnSave.removeClass("pure-menu-disabled");
                btnCancel.removeClass("pure-menu-disabled");
                chkPublish.removeAttr("disabled");
                chkComments.removeAttr("disabled");

                btnDrafts.addClass("hidden");
                btnNew.addClass("hidden");
                btnEdit.addClass("hidden");
                btnSave.removeClass("hidden");
                btnCancel.removeClass("hidden");
                chkPublish.parent().parent().removeClass("hidden");
                chkComments.parent().parent().removeClass("hidden");
                 
                $("#date").hide();
                $("#comments").hide();
                $("#commentform").hide();
            }).fail(function(data) {
                alert("failed to load post data." + data);
            });
        },
        cancelEdit = function() {
            if (isNew) {
                if (confirm(Resources.SureLeave))
                    history.go(-2);
            } else {
                if (confirm(Resources.SureLeave)) {
                    exitFromEditing();
                }
            }
        },
        exitFromEditing = function() {

            btnCancel.focus();
            $(".articleBody").show();
            showTitleAndCategoriesForDisplay();
            $("#contentWrapper").empty();

            btnDrafts.removeClass("pure-menu-disabled");
            btnNew.removeClass("pure-menu-disabled");
            btnEdit.removeClass("pure-menu-disabled");
            btnSave.addClass("pure-menu-disabled");
            btnCancel.addClass("pure-menu-disabled");
            chkPublish.attr("disabled");
            chkComments.attr("disabled");

            btnDrafts.removeClass("hidden");
            btnNew.removeClass("hidden");
            btnEdit.removeClass("hidden");
            btnSave.addClass("hidden");
            btnCancel.addClass("hidden");
            chkComments.parent().addClass("hidden");
            chkPublish.parent().addClass("hidden");

             
            $("#date").show();
            $("#comments").show();
            if (chkComments[0].checked)
                $("#commentform").show();

            location.href = "#";
        },
        saveAndExit = function(e) { savePost(e, true); },
        saveAndStay = function() { savePost(null, false); },
        savePost = function(e, doExit) {
            if (editor === undefined)
                return;

            if (doExit === undefined)
                doExit = true;

            var content = editor.getValue();
            var renderedContent = converter.makeHtml(content);

            var title = $("#postTitle").val().trim();
            if (title.length == 0) {
                if (isNew) {
                    showMessage(false, Resources.SlugNotEmpty);
                } else {
                    showMessage(false, Resources.TitleNotEmpty.TitleNotEmpty);
                }
                return;
            }
            if (content.trim().length == 0) {
                showMessage(false, Resources.ContentNotEmpty);
                return;
            }
            $.post("/post.ashx?mode=save", {
                    id: postId,
                    isPublished: chkPublish[0].checked,
                    isComments: chkComments[0].checked,
                    title: title,
                    content: content,
                    categories: getPostCategories(),
                })
                .success(function(data) {
                    if (isNew) {
                        if (doExit) {
                            location.href = data;
                            return;
                        } else {
                            location.href = data + "#/edit";
                        }
                    }
                    showMessage(true, Resources.PostSaved);

                    if (doExit) {
                        exitFromEditing();
                        renderPost(renderedContent);
                    }
                })
                .fail(function(data) {
                    if (data.status === 409)
                        showMessage(false, Resources.TitleInUse);
                    else
                        showMessage(false, Resources.SomethingBadHappened + data.status + " " + data.statusText);
                });
        },
        deletePost = function() {
            if (confirm(Resources.SureDeletePost)) {
                $.post("/post.ashx?mode=delete", { id: postId })
                    .success(function() { location.href = "/"; })
                    .fail(function() { showMessage(false, Resources.SomethingWentWrong); });
            }
        },
        showMessage = function(success, message) {
            var bannerHead = $(".banner-head a"),
                className = success ? "alert-success" : "alert-danger",
                documentWidth, txtMessageWidth, bannerHeadWidth = 0;
            
            txtMessage.addClass(className);
            txtMessage.text(message);
            txtMessage.parent().fadeIn();

            documentWidth = $(document).width();
            txtMessageWidth = txtMessage.width();  

            bannerHead.each(function () {
                bannerHeadWidth += parseInt($(this).width(), 10);
            });

            var hideBannerHead = documentWidth - txtMessageWidth - bannerHeadWidth < 0;

            if (hideBannerHead) {
                bannerHead.fadeOut("slow");
            }

            $(".changeLanguage").fadeOut("slow");

            setTimeout(function() {
                txtMessage.parent().fadeOut("slow", function() {
                    txtMessage.removeClass(className);
                });

                if (hideBannerHead) {
                    bannerHead.fadeIn("slow");
                }

                $(".changeLanguage").fadeIn("slow");
            }, 4000);
        },
        getPostCategories = function() {
            var categories = '';
            if ($("#txtCategories").length > 0) {
                categories = $("#txtCategories").val();
            } else {
                $("ul.categories li a").each(function(index, item) {
                    if (categories.length > 0) {
                        categories += ",";
                    }
                    categories += $(item).html();
                });
            }
            return categories;
        },
        showTitleAndCategoriesForEditing = function() {
            var categoriesString = getPostCategories();

            if (isNew) {
                $('.postFooter').hide();
                $('#postTitle').attr('placeholder', Resources.Slug);
                $('#postTitle').on("keypress paste", function(event) {
                     
                    if (event.ctrlKey || event.altKey) return;
                    if (event.type == 'keypress') {

                        var key = event.charCode ? event.charCode : event.keyCode ? event.keyCode : 0;

                        // 8 = backspace, 9 = tab, 13 = enter, 35 = end, 36 = home, 37 = left, 39 = right, 46 = delete
                        if (key == 8 || key == 9 || key == 13 || key == 35 || key == 36 || key == 37 || key == 39 || key == 46) {

                            // if charCode = key & keyCode = 0
                            // 35 = #, 36 = $, 37 = %, 39 = ', 46 = .

                            if (event.charCode == 0 && event.keyCode == key) {
                                return true;
                            }
                        }
                    }

                    setTimeout(function () {
                        $('#postTitle').val(makeSlug($('#postTitle').val()));
                    }, 1500);
                });
            }

            $("#postTitle").val(txtTitle.html().trim());
            $("#postTitle").focus();
            txtTitle.hide();

            $("ul.categories li").each(function(index, item) {
                $(item).remove();
            });

            $("#txtCategories").val(categoriesString);

            loadCategories(applySelectize);
        },
        makeSlug = function( str ){
            str = str.toLowerCase();
            str = str.replace(/[^a-z0-9-]+/g, '');
            str = str.replace(/^-|-$/g, '');
            return str;
        }, 
        showTitleAndCategoriesForDisplay = function() {
            txtTitle.html($("#postTitle").val());
            txtTitle.show();

            if ($("#txtCategories").length > 0) {
                var categoriesArray = $("#txtCategories").val().split(',');
                $("#txtCategories").parent().parent().remove();

                $.each(categoriesArray, function(index, category) {
                    $("ul.categories").append(' <li itemprop="articleSection" title="' +
                        category +
                        '"> <a href="/category/' + encodeURIComponent(category.toLowerCase()) +
                        '">' + category + '</a> </li> ');
                });
            }
        },
        loadCategories = function(callback) {
            $.ajax({
                url: '/post.ashx?mode=categories',
                type: 'POST',
                error: function(res) {
                    callback(res);
                },
                success: function(res) {
                    callback(res.repositories);
                }
            });
        },
        applySelectize = function(categories) {
            $('#txtCategories').selectize({
                delimiter: ',',
                valueField: 'name',
                labelField: 'name',
                searchField: 'name',
                create: function(input) {
                    return {
                        name: input
                    };
                },
                persist: false,
                options: categories,
                render: {
                    item: function(item, escape) {
                        return '<div><span class="name">' + escape(item.name) + '</span></div>';
                    },
                    option: function(item, escape) {
                        return '<div><span class="name">' + escape(item.name) + '</span></div>';
                    }
                },
            });
        },
        requireTemplate = function(templateName) {
            var template = $('#template_' + templateName);
            if (template.length === 0) {
                var tmplDir = '/admin/templates';
                var tmplUrl = tmplDir + '/' + templateName + '.html';
                var tmplString = '';

                $.ajax({
                    url: tmplUrl,
                    method: 'GET',
                    async: false,
                    contentType: 'text',
                    success: function(data) {
                        tmplString = data;
                    }
                });

                $('head').append('<script id="template_' +
                    templateName + '" type="text/template">' + tmplString + '<\/script>');
            }
        },
        syncScroll = function(e) {
            // Sync scrolling

            // vars
            var $codeViewport = $(e.target),
                $previewViewport = $('.entry-preview-content'),
                $codeContent = $('.CodeMirror-sizer'),
                $previewContent = $('.rendered-markdown'),
                // calc position
                codeHeight = $codeContent.height() - $codeViewport.height(),
                previewHeight = $previewContent.height() - $previewViewport.height(),
                ratio = previewHeight / codeHeight,
                previewPostition = $codeViewport.scrollTop() * ratio;

            // apply new scroll
            $previewViewport.scrollTop(previewPostition);
        },
        renderPost = function(renderedContent) {
            $(".articleBody")
                .html(renderedContent)
                .show();

            $("#contentWrapper").empty();
            App.applyHighlightJs();
            App.applyMagnificPopup();
        },
        updateWordCount = function() {
            // Really not the best way to do things as it includes
            // Markdown formatting along with words

            var wordCount = document.getElementsByClassName('entry-word-count')[0],
                editorValue = editor.getValue();

            if (editorValue.length) {
                wordCount.innerHTML = editorValue.match(/\S+/g).length + '';
            }
        },
        updatePreview = function() {
            var preview = document.getElementsByClassName('rendered-markdown')[0];
            preview.innerHTML = converter.makeHtml(editor.getValue());

            App.applyHighlightJs();
            updateWordCount();
        },
        initEditor = function(data) {
            requireTemplate("editor");
            var template = _.template($("#template_editor").html());
            $(".articleBody").hide();
            $("#contentWrapper").append(template);

            if (!document.getElementById('entry-markdown'))
                return;

            converter = new Markdown.Converter();
            Markdown.Extra.init(converter);

            editor = CodeMirror.fromTextArea(document.getElementById('entry-markdown'), {
                mode: 'markdown',
                tabMode: 'indent',
                lineWrapping: true,
            });
            editor.setValue(data);

            inlineAttach.attachToCodeMirror(editor, { uploadUrl: '/post.ashx?mode=saveImage&id=' + postId });

            $('.entry-markdown header, .entry-preview header').click(function(e) {
                $('.entry-markdown, .entry-preview').removeClass('active');
                $(e.target).closest('section').addClass('active');
            });

            editor.on("change", function() {
                updatePreview();
            });

            updatePreview();

            // TODO: Debounce
            $('.CodeMirror-scroll').on('scroll', syncScroll);

            // Shadow on Markdown if scrolled
            $('.CodeMirror-scroll').scroll(function() {
                if ($('.CodeMirror-scroll').scrollTop() > 10) {
                    $('.entry-markdown').addClass('scrolling');
                } else {
                    $('.entry-markdown').removeClass('scrolling');
                }
            });
            // Shadow on Preview if scrolled
            $('.entry-preview-content').scroll(function() {
                if ($('.entry-preview-content').scrollTop() > 10) {
                    $('.entry-preview').addClass('scrolling');
                } else {
                    $('.entry-preview').removeClass('scrolling');
                }
            });

            $(window).resize(function() {
                var width = $(window).width();
                var margin = 360;
                if (width < 860)
                    margin = 390;
                if (width < 767)
                    margin = 380;

                $(".features .editor .editorwrap").height($(window).height() - margin);
            });

            $(window).trigger('resize');

            $('.open-popup-link').magnificPopup({
                type: 'inline',
                midClick: true // allow opening popup on middle mouse click. Always set it to true if you don't provide alternative source.
            });
        },
        initEditorShortcuts = function() {
            var markdownShortcuts = [
                { 'key': 'Ctrl+B', 'style': 'bold' },
                { 'key': 'Meta+B', 'style': 'bold' },
                { 'key': 'Ctrl+I', 'style': 'italic' },
                { 'key': 'Meta+I', 'style': 'italic' },
                { 'key': 'Ctrl+Alt+U', 'style': 'strike' },
                { 'key': 'Ctrl+Shift+K', 'style': 'code' },
                { 'key': 'Meta+K', 'style': 'code' },
                { 'key': 'Ctrl+Alt+1', 'style': 'h1' },
                { 'key': 'Ctrl+Alt+2', 'style': 'h2' },
                { 'key': 'Ctrl+Alt+3', 'style': 'h3' },
                { 'key': 'Ctrl+Alt+4', 'style': 'h4' },
                { 'key': 'Ctrl+Alt+5', 'style': 'h5' },
                { 'key': 'Ctrl+Alt+6', 'style': 'h6' },
                { 'key': 'Ctrl+Shift+L', 'style': 'link' },
                { 'key': 'Ctrl+Shift+I', 'style': 'image' },
                { 'key': 'Ctrl+Q', 'style': 'blockquote' },
                { 'key': 'Ctrl+U', 'style': 'uppercase' },
                { 'key': 'Ctrl+Shift+U', 'style': 'lowercase' },
                { 'key': 'Ctrl+Alt+Shift+U', 'style': 'titlecase' },
                { 'key': 'Ctrl+Alt+W', 'style': 'selectword' },
                { 'key': 'Ctrl+L', 'style': 'list' }
            ];

            _.each(markdownShortcuts, function(combo) {
                shortcut.add(combo.key, function() {
                    return editor.addMarkdown({ style: combo.style });
                });
            });

            shortcut.add('Ctrl+S', function() {
                saveAndStay();
            });

            shortcut.add('Ctrl+Enter', function() {
                saveAndExit();
                initShortcuts();
            });

            shortcut.add('F1', function() {
                $('.open-popup-link').magnificPopup('open');
            });

            shortcut.remove("F2");
        },
        initShortcuts = function() {
            shortcut.add('F2', function() {
                $("#btnEdit a").trigger("click");
            });

            shortcut.remove("F1");
            shortcut.remove("Ctrl+S");
            shortcut.remove("Ctrl+Enter");
        },
        initVariablesAndLayout = function() {
            isNew = location.pathname.replace(/\//g, "") === "lognew";
            postId = $("[itemprop~='blogPost']").attr("data-id");
            txtTitle = $("[itemprop~='blogPost'] [itemprop~='name']");
            txtMessage = $("#alert .alert");
            btnDrafts = $("#btnDrafts");
            btnNew = $("#btnNew");
            btnEdit = $("#btnEdit");
            btnDelete = $("#btnDelete").bind("click", deletePost);
            btnSave = $("#btnSave").bind("click", function() {
                saveAndExit();
                initShortcuts();
            });
            btnCancel = $("#btnCancel").bind("click", cancelEdit);
            chkPublish = $("#ispublished").find("input[type=checkbox]");
            chkComments = $("#iscomments").find("input[type=checkbox]");

            if (isNew) {
                chkPublish[0].checked = true;
                chkComments[0].checked = true;
            } else if (txtTitle !== null && txtTitle.length === 1 && location.pathname.length > 1) {
                btnEdit.removeClass("hidden");
                btnDelete.removeClass("hidden");
            }

            $("#ispublished").css({ "display": "inline-block" });
            $("#iscomments").css({ "display": "inline-block" });
        },
        initPaths = function() {
            Path.map("#/edit").to(function() {
                isEdited = true;
                editPost();
                initEditorShortcuts();
            });

            Path.map("#/index").to(function() {
                initShortcuts();

                if (isEdited) {
                    exitFromEditing();
                }
            });

            //Path.root("#/index");

            Path.listen();
        };

    initShortcuts();
    initVariablesAndLayout();
    initPaths();

})(jQuery, CodeMirror, Path, App, Resources);