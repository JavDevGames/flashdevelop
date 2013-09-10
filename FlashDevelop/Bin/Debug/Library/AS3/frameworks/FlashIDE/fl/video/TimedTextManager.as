﻿package fl.video
{
	import flash.net.URLLoader;
	import flash.net.URLRequest;
	import flash.events.*;

	/**
	 * <p>Handles downloading and parsing Timed Text (TT) xml format
	 */
	public class TimedTextManager
	{
		local var owner : FLVPlaybackCaptioning;
		private var flvPlayback : FLVPlayback;
		private var videoPlayerIndex : uint;
		private var limitFormatting : Boolean;
		public var xml : XML;
		public var xmlLoader : URLLoader;
		private var _url : String;
		local var nameCounter : uint;
		local var lastCuePoint : Object;
		local var styleStack : Array;
		local var definedStyles : Object;
		local var styleCounter : uint;
		local var whiteSpacePreserved : Boolean;
		local var fontTagOpened : Object;
		local var italicTagOpen : Boolean;
		local var boldTagOpen : Boolean;
		static var CAPTION_LEVEL_ATTRS : Array;
		local var xmlNamespace : Namespace;
		local var xmlns : Namespace;
		local var tts : Namespace;
		local var ttp : Namespace;

		/**
		 * constructor
		 */
		public function TimedTextManager (owner:FLVPlaybackCaptioning);
		/**
		 * <p>Starts download of XML file.  Will be parsed and based
		 */
		function load (url:String) : void;
		/**
		 * <p>Handles load of XML.
		 */
		function xmlLoadEventHandler (e:Event) : void;
		/**
		 * parse head node of tt
		 */
		function parseHead (parentNode:XML) : void;
		/**
		 * parse styling node of tt
		 */
		function parseStyling (parentNode:XML) : void;
		/**
		 * parse body node of tt
		 */
		function parseBody (parentNode:XML) : void;
		function parseParagraph (parentNode:XML) : void;
		function parseSpan (parentNode:XML, cuePoint:Object) : String;
		function openFontTag () : String;
		function closeFontTags () : String;
		function parseStyleAttribute (xmlNode:XML, styleObj:Object) : void;
		/**
		 * Extracts supported style attributes (tts:... attributes
		 */
		function parseTTSAttributes (xmlNode:XML, styleObject:Object) : void;
		function getStyle () : Object;
		function pushStyle (styleObj:Object) : void;
		function popStyle () : void;
		/**
		 * copies attributes from one style object to another
		 */
		function copyStyleObjectProps (tgt:Object, src:Object) : void;
		/**
		 * parses color
		 */
		function parseColor (colorStr:String) : Object;
		/**
		 * parses fontSize
		 */
		function parseFontSize (sizeStr:String) : String;
		/**
		 * parses fontFamily
		 */
		function parseFontFamily (familyStr:String) : String;
		/**
		 * parse time in hh:mm:ss.s or mm:ss.s format.
		 */
		function parseTimeAttribute (parentNode:XML, attr:String, req:Boolean) : Number;
		/**
		 * checks for extra, unrecognized elements of the given kind
		 */
		function checkForIllegalElements (parentNode:XML, legalNodes:Object) : void;
		/**
		 * @private
		 */
		function fixCaptionText (inText:String) : String;
		/**
		 * @private
		 */
		function unicodeEscapeReplace (match:String, first:String, second:String, index:int, all:String) : String;
		/**
		 * @private
		 */
		function getSpaceAttribute (node:XML) : String;
	}
}