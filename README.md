Blog engine. Combination of 
[Ghost Markdown Editor](https://github.com/timsayshey/Ghost-Markdown-Editor) and
[MiniBlog](https://github.com/madskristensen/miniblog) projects.

![Blog](http://anvslv.me/images/blog-en-thumb.png "Blog")
 
Language is set in web.config. 

	<appSettings> 
		<add key="blog:lang" value="ru" /> 
	</appSettings>

If this settings is not in web.config, language can be changed with gui and is persisted with help of cookie.
Server side uses [MarkdownDeep][5], [ImageResizer][6]. 
The core of client side is [Ghost][7] editor. ALso, [highlight.js][8], 
[magnific popup][9], [pure.css][10], [codemirror.inline-attach][11], [pagedown-extra][12], 
[selectize][13] [underscore][14], [path][15] are used.

Press <kbd>F2</kbd> to begin editing current post.
Keyboard shortcuts in editing mode:
  
* <kbd>F1</kbd> — editing help
* <kbd>Ctrl+S</kbd> — save
* <kbd>Ctrl+Enter</kbd> — save and exit editing mode
 
[1]: https://github.com/timsayshey/Ghost-Markdown-Editor 
[2]: https://github.com/madskristensen/miniblog 
[3]: http://anvslv.me/Images/v-635266277710373031/blog-thumb.png 
[4]: http://anvslv.me/Images/v-635266277710217040/blog-help-thumb.png
[5]: https://github.com/toptensoftware/markdowndeep
[6]: http://imageresizing.net/ 
[7]: https://github.com/TryGhost/Ghost 
[8]: https://github.com/isagalaev/highlight.js 
[9]: http://dimsemenov.com/plugins/magnific-popup/ 
[10]: http://purecss.io 
[11]: https://github.com/Rovak/InlineAttachment 
[12]: https://github.com/jmcmanus/pagedown-extra
[13]: https://github.com/brianreavis/selectize.js 
[14]: http://underscorejs.org/
[15]: https://github.com/mtrpcic/pathjs