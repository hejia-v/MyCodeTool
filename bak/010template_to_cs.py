import os
import re
from collections import namedtuple

# https://python3-cookbook.readthedocs.io/zh_CN/latest/c02/p19_writing_recursive_descent_parser.html

BRACKET_LEFT = r'(?P<BRACKET_LEFT>[(])'
BRACKET_RIGHT = r'(?P<BRACKET_RIGHT>[)])'

MIDDLE_BRACKET_LEFT = r'(?P<MIDDLE_BRACKET_LEFT>[\[])'
MIDDLE_BRACKET_RIGHT = r'(?P<MIDDLE_BRACKET_RIGHT>[\]])'

CURLY_BRACE_LEFT = r'(?P<CURLY_BRACE_LEFT>[\{])'
CURLY_BRACE_RIGHT = r'(?P<CURLY_BRACE_RIGHT>[\}])'



STR_TEXT = r'(?P<STR_TEXT>"([^"]*)")'
WS = r'(?P<WS>\s+)'

DEFINE = r'(?P<DEFINE>\#define)'
INCLUDE = r'(?P<INCLUDE>\#include)'
TYPEDEF = r'(?P<TYPEDEF>typedef)'
STRUCT = r'(?P<STRUCT>struct\b)'



COLON = r'(?P<COLON>:)'
SEMICOLON = r'(?P<SEMICOLON>;)'
COMMA = r'(?P<COMMA>,)'
DOT = r'(?P<DOT>\.)'
OR = r'(?P<OR>\|\|)'
AND_SYMBOL = r'(?P<AND_SYMBOL>&)'
DOLLER = r'(?P<DOLLER>\$)'
PERCENT = r'(?P<PERCENT>%)'
MINUS = r'(?P<MINUS>\-)'
PLUS = r'(?P<PLUS>\+)'
DIVIDE = r'(?P<DIVIDE>\/)'
AT = r'(?P<AT>@)'
GREATER = r'(?P<GREATER>>)'
SMALLER = r'(?P<SMALLER><)'
UP_ARROW = r'(?P<UP_ARROW>\^)'
TIMES = r'(?P<TIMES>\*)'
EXCLAMATION = r'(?P<EXCLAMATION>!)'
QUESTION = r'(?P<QUESTION>\?)'
EQ_COMP = r'(?P<EQ_COMP>==)'
EQ = r'(?P<EQ>=)'


HEX = r'(?P<HEX>0[xX][0-9a-fA-F]+)'
NUMBERS = r'(?P<NUMBERS>[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?[f]?)'
NUM = r'(?P<NUM>\d+)'

VARIABLE = r'(?P<VARIABLE>\b[A-Za-z0-9_]+\b)'

master_pat = re.compile('|'.join([
    BRACKET_LEFT, BRACKET_RIGHT,CURLY_BRACE_LEFT,CURLY_BRACE_RIGHT,MIDDLE_BRACKET_LEFT,MIDDLE_BRACKET_RIGHT,
    STR_TEXT,
    WS,
    DEFINE,
    INCLUDE,
    TYPEDEF,
    STRUCT,

    COLON,SEMICOLON,COMMA,DOT,OR,AND_SYMBOL,DOLLER,PERCENT,MINUS,AT,PLUS,DIVIDE,GREATER,UP_ARROW,TIMES,SMALLER,
    HEX,
    NUMBERS,
    # SEND,NULL,STR, BEGIN, CONST,GVASGN,HASH,PAIR,SYM,LVASGN,BLOCK,ARGS,PROCARG0,ARG,LVAR,
    # INDEXASGN,GVAR,DSTR,INDEX,INT,MODULE,CLASS,DEFS,DEF,IVASGN,SUPER,IVAR,SELF,
    EQ_COMP, EQ,EXCLAMATION, QUESTION,
    # REQUIRE_RELATIVE, REQUIRE,INCLUDE,OPTIONS,
    # REF_PTR,
    # IF,
    VARIABLE, NUM,
    ]))

def generate_tokens(pat, text):
    # print(text)
    Token = namedtuple('Token', ['type', 'value'])
    scanner = pat.scanner(text)
    for m in iter(scanner.match, None):
        yield Token(m.lastgroup, m.group())

class FieldNode:
    pass

class SentenceNode:
    pass

class FunctionNode:
    pass

class MainProgramNode:
    pass
    def __init__(self):
        self.structs = []


class TokenReader:
    def __init__(self, tokens):
        self.tokens = tokens
        self.position = 0
        pass

    def advance(self):
        self.position += 1
        pass

    def read(self):
        if self.position < len(self.tokens):
            token = self.tokens[self.position]
            self.position += 1
            return token
        return None
    
    def peak(self):
        if self.position < len(self.tokens):
            token = self.tokens[self.position]
            return token
        return None

    def back(self):
        if self.position > 0:
            self.position -= 1

    def can_read(self):
        return self.position < len(self.tokens)

    def is_end(self):
        return not self.can_read()
        pass

    def seek_to_Sentence_end(self):
        while self.can_read():
            tok = self.read()
            if tok.type == 'SEMICOLON':
                return

class StructNode:
    def __init__(self):
        pass

    def parse(self, reader:TokenReader):
        pos = reader.position

        tok = reader.read()
        assert tok.type == 'CURLY_BRACE_LEFT'
        brace_depth = 1

        # paser body
        while self.reader.can_read():
            tok = self.reader.read()
            if tok.type == 'CURLY_BRACE_LEFT':
                brace_depth += 1
            elif tok.type == 'CURLY_BRACE_RIGHT':
                brace_depth -= 1
                if brace_depth == 0:
                    break
            else:
                pass
        # parser name
        pass


class Generator:
    def __init__(self):
        self.is_typedef = False
        pass

    def parse(self, tokens):
        self.reader = TokenReader(tokens)
        self.is_typedef = False
        self.main_program = MainProgramNode()

        while self.reader.can_read():
            tok = self.reader.read()
            if tok.type == 'DEFINE':
                self.reader.seek_to_Sentence_end()
                # break
                continue
            elif tok.type == 'TYPEDEF':
                self.is_typedef = True
                tok = self.reader.read()
                assert tok.type == 'STRUCT'
                tok = self.reader.read()
                assert tok.type == 'CURLY_BRACE_LEFT'
                self.reader.back()
                struct_node = StructNode()
                struct_node.parse(self.reader)
                self.main_program.structs.append(struct_node)
                pass
            else:
                break
            pass
    
    def _advance(self):
        pass


def main():
    ast_file = 'E:/dev/bayonetta_tools/binary_templates/Bayonetta wmb.bt'
    # ast_file = 'E:/dev/bayonetta_tools/binary_templates/Bayonetta wmb base.bt'
    with open(ast_file, 'rt') as fd:
        ast_text = fd.read()
    # print(ast_text)
    tmp_lst = []
    tokens = []
    for tok in generate_tokens(master_pat, ast_text):
        tmp_lst.append(tok.value)
        if tok.type != 'WS':
            print(tok)
            tokens.append(tok)
    pass
    tmp_txt = ''.join(tmp_lst)
    print(tmp_txt)
    print('last line:', len(tmp_txt.split('\n')))

    g = Generator()
    g.parse(tokens)

# 
if __name__ == '__main__':
    main()
