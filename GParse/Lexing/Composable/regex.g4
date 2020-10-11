main
	: main '|' sequence
	| sequence
	;

sequence
	: suffixed*
	;

suffixed
	: repetition
	| expression
	;

repetition
	: expression ('?' | '*' | '+' | '{' NUMBER '}' | '{' NUMBER? ',' NUMBER? '}') '?'?
	;

expression
	: alternation_set
	| lookahead
	| non_capturing_group
	| named_capture_group
	| numbered_capture_group
	| backreference
	| character_class
	| CHAR
	;

named_capture_group
	: '(?<' WORD_CHAR+ '>' main ')'
	;

numbered_capture_group
	: '(' main ')'
	;

backreference
	: '\\' ('0'..'9')+
	| '\\k<' WORD_CHAR+ '>'
	;

non_capturing_group
	: '(?:' main ')'
	;

lookahead
	: '(?' ('=' | '!') main ')'
	;

alternation_set
	: '[' '^'? ']'? alternation_element+ ']'
	;

alternation_element
	: character_class
	| character_range
	| ALTERNATION_CHAR
	;

character_range
	: ALTERNATION_CHAR '-' ALTERNATION_CHAR
	;

ALTERNATION_CHAR
	: ESCAPE
	| [^\]\\]
	;

CHAR
	: ESCAPE
	| ~SPECIAL_CHAR
	;

character_class
	: '\\' (
		'd'
		| 'D'
		| 'w'
		| 'W'
		| 's'
		| 'S'
	)
	| UNICODE_CLASS_OR_BLOCK
	;

UNICODE_CLASS_OR_BLOCK:
	: '\\' ('p' | 'P') '{' UNICODE_CLASS_OR_BLOCK_NAME '}'
	;

UNICODE_CLASS_OR_BLOCK_NAME
	: (WORD_CHAR | '-')+ /* <any of the names in UnicodeCharacterCategoriesAndCodeBlocks.xml> */
	;

ESCAPE
	: HEX_ESCAPE
	| SIMPLE_ESCAPE
	;

HEX_ESCAPE
	: '\\x' HEX_DIGIT+
	;

SIMPLE_ESCAPE
	: '\\a'
/*  | '\\b' */
	| '\\f'
	| '\\n'
	| '\\r'
	| '\\t'
	| '\\v'
	| '\\' SPECIAL_CHAR
	;

SPECIAL_CHAR
	: [.$^{\[(|)*+?\\]
	;

NUMBER
	: [0-9]+
	;

WORD_CHAR
	: [a-zA-Z0-9_]
	;

HEX_DIGIT
	: [a-fA-F0-9]
	;