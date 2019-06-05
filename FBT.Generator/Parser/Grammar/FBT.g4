
grammar FBT;

frostbiteType
    : includeBlock typeData*
    | typeData+ EOF
    ;

includeBlock
    : include*
    ;

include
    : '#' 'include' QUOTED_IMPORT
    ;

typeData
    : typeNamespace
    | typeModule
    | typeClass
    | typeEnum
    | typeValueType
    | typePrimitive
    ;


typeNamespace
    : 'namespace' UNQUOTED_STRING ';'
    ;

typeModule
    : 'module' UNQUOTED_STRING ';'
    ;

classAttributes
    : typeAttribute (',' typeAttribute)*
    ;

typeAttributes
    : '[' typeAttribute (',' typeAttribute)* ']'
    ;

typeAttribute
    : numeralTypeAttribute
    | basicTypeAttribute
    ;

basicTypeAttribute
    : UNQUOTED_STRING
    ;

numeralTypeAttribute
    : UNQUOTED_STRING '(' numeral ')'
    ;

// ---------------------- Enum ----------------------------

typeEnum
    : typeAttributes? 'enum' enumName '{' enumData* '}'
    ;

enumName
    : UNQUOTED_STRING
    ;

enumData
    : enumValuePair (',' enumValuePair)*
    ;

enumValuePair
    : UNQUOTED_STRING '=' numeral
    ;


// ---------------------- Class ----------------------------

typeClass
    : typeAttributes? 'class' classAttributes? classTypeName classInherrits? '{' structureData* '}'
    ;



classTypeName
    : UNQUOTED_STRING
    ;

classInherrits
    : ':' classInherrit (',' classInherrit)*
    ;

classInherrit
    : UNQUOTED_STRING
    ;




// ---------------------- ValueType ----------------------------

typeValueType
    : typeAttributes? valueTypeIdentifier classAttributes? valueTypeName  '{' structureData* '}'
    ;

valueTypeIdentifier
	: 'valuetype'
	| 'struct'
	;

valueTypeName
    : UNQUOTED_STRING
    ;


//--------------- StructureData -----------

structureData
    : memberData ';'
    ;


memberData
    : typeAttributes? memberVisibility? memberTypeName memberArrayType? memberName ':' memberOffset
    ;

memberVisibility
    : 'public'
    ;


memberTypeName
    : UNQUOTED_STRING
    ;

memberArrayType
    : '[' ']'
    | '[' numeral ']'
    ;

memberName
    : UNQUOTED_STRING
    ;



memberOffset
    : numeral
    ;

// ---------------------- PrimitiveType ----------------------------

typePrimitive
    : typeAttributes 'primitive' primitiveTypeName
    ;

primitiveTypeName
    : UNQUOTED_STRING
    ;


numeral /*Consists of integer, float, long, hex*/
    : NUMBER
    | FLOAT
    | HEX
    ;

NUMBER
    : '-'? DecDigit+
    ;

FLOAT
    : NUMBER '.' DecDigit* FloatExponent?
    | NUMBER FloatExponent
    ;


UNQUOTED_STRING
    :  Nondigit (Nondigit | DecDigit)*
    ;

QUOTED_STRING
    : '"' (~[\r\n\\"] | EscChar)* '"'
    | '\'' (~[\r\n\\'] | EscChar)* '\''
    ;

QUOTED_IMPORT
    : '<' (~[\r\n\\"] | EscChar)* '>'
    ;

fragment Nondigit
    :   [a-zA-Z_]
    ;

fragment HexDigit
    : [0-9a-fA-F]
    ;

fragment DecDigit
    : [0-9]
    ;

fragment HexByte
    : HexDigit HexDigit
    ;

fragment FloatExponent
    : [eE] [+-]? DecDigit+
    ;

fragment EscChar
    : '\\\n'
    | '\\' [\\abfnrtvz'"]
    | '\\x' HexDigit HexDigit
    | '\\' DecDigit DecDigit? DecDigit?
    | '\\u{' HexDigit+ '}'
    | '\\z' [ \t\r\n]+
    ;


CommentDirective
    : '//' ~[\n]* -> skip
    ;

WS
    : [\r\n\t ]+ -> skip
    ;