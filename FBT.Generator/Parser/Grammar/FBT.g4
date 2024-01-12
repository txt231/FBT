
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
    | typeArray
    | typeInterface
    | typeFunction
    | typeDelegate
    | typeInterface
    | typePrimitive
    ;

typeNamespace
    : 'namespace' UNQUOTED_STRING ';'
    ;

typeModule
    : 'module' UNQUOTED_STRING ';'
    ;





// ---------------------- Enum ----------------------

typeEnum
    : typeAttributes? 'enum' classAttributes? enumName '{' enumData* '}'
    ;

enumName
    : UNQUOTED_STRING
    ;

enumData
    : enumValuePair ';'
    ;

enumValuePair
    : typeAttributes? UNQUOTED_STRING '=' numeral
    ;






// ---------------------- Class ----------------------

typeClass
    : typeAttributes? 'class' classAttributes? classTypeName classInherrits? '{' baseClassVariable* structureData* '}'
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

// ---------------------- Array ----------------------

typeArray
    : typeAttributes? 'array' classAttributes? arrayTypeName arrayRef? ';'
    ;

arrayTypeName
    : UNQUOTED_STRING
    ;

arrayRef
    : '->' arrayRefName
    ;

arrayRefName
    : UNQUOTED_STRING
    ;


// ---------------------- Function ----------------------

typeDelegate
    : typeAttributes? 'delegate' classAttributes? classTypeName '{' functionData* '}'
    ;

typeFunction
    : typeAttributes? 'function' classAttributes? classTypeName '{' functionData* '}'
    ;


functionData
    : functionParamData ';'
    ;

functionParamData
    : functionParamType functionParamTypeName functionParamName ('=' memberValue)?
    ;

functionParamType
    : 'in'
    | 'out'
    | 'inref'
    | 'outref' 
    | 'unk'
    ;

functionParamTypeName
    : UNQUOTED_STRING
    ;

functionParamName
    : UNQUOTED_STRING
    ;


// ---------------------- Interface ----------------------

typeInterface
    : typeAttributes? 'interface' classAttributes? classTypeName ';'
    ;

// ---------------------- ValueType ----------------------

typeValueType
    : typeAttributes? valueTypeIdentifier classAttributes? valueTypeName  '{' baseClassVariable* structureData* '}'
    ;

valueTypeIdentifier
	: 'valuetype'
	| 'struct'
	;

valueTypeName
    : UNQUOTED_STRING
    ;


// ---------------------- StructureData ----------------------

baseClassVariable
    : baseClassName '.' baseFieldName '=' memberValue ';'
    ;

baseClassName
    : UNQUOTED_STRING
    ;
baseFieldName
    : UNQUOTED_STRING
    ;


structureData
    : memberData ';'
    ;


memberData
    : typeAttributes? memberVisibility? memberTypeName memberArrayType? memberName ':' memberOffset memberDefaultValue?
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


memberDefaultValue
    : '=' memberValue
    ;

memberValue
    : '{' memberInstanceField* '}'      # MemberValueInstance
    | '[' memberArrayElement* ']'       # MemberValueArray
    | numeral                           # MemberValueNumber
    | float                             # MemberValueFloat
    | QUOTED_STRING                     # MemberValueString
    | 'null'                            # MemberValueNull

//    | '{' numeral (',' numeral)* '}'    // byearray
    ;

memberInstanceField
    : UNQUOTED_STRING '=' memberValue ','
    ;
    
memberArrayElement
    : memberValue
    ;

/*
memberBoolean
    : Boolean
    ;
    */

// ---------------------- PrimitiveType ----------------------

typePrimitive
    : typeAttributes? 'primitive' classAttributes? primitiveTypeName ';'
    ;

primitiveTypeName
    : UNQUOTED_STRING
    ;

// ---------------------- Attributes ----------------------
classAttributes
    : typeAttribute (',' typeAttribute)*
    ;

typeAttributes
    : typeAttributesData+
    ;

typeAttributesData
    : '[' (typeAttribute (',' typeAttribute)*)? ']'
    ;

typeAttribute
    : numeralTypeAttribute
    | stringTypeAttribute
    | basicTypeAttribute
    ;

basicTypeAttribute
    : UNQUOTED_STRING
    ;

numeralTypeAttribute
    : UNQUOTED_STRING '(' numeral ')'
    ;

stringTypeAttribute
    : UNQUOTED_STRING '(' QUOTED_STRING ')'
    ;

numeral /*Consists of integer, float, long, hex*/
    : NUMBER
    | HEX
    ;
    
float
    : FLOAT
    ;


QUOTED_STRING
    : '"' (~[\r\n\\"] | EscChar)* '"'
    | '\'' (~[\r\n\\'] | EscChar)* '\''
    ;

QUOTED_IMPORT
    : '<' (~[\r\n\\"] | EscChar)* '>'
    ;

HEX
    : [-]? '0x' HexDigit+
    ;

NUMBER
    : [-]? DecDigit+
    ;


FLOAT
    : NUMBER '.' DecDigit* FloatExponent?
    | NUMBER FloatExponent?
    ;



fragment Nondigit
    : ([a-zA-Z_] | [-])
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


UNQUOTED_STRING
    :  Nondigit (Nondigit | DecDigit | HexDigit)*
    ;

/* 
fragment Boolean
    : [tT][r][u][e]
    | [fF][a][l][s][e]
    ;
*/

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