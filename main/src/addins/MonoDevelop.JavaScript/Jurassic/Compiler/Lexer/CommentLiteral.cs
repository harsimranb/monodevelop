using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jurassic.Compiler
{
    public class CommentLiteral
    {
        public CommentLiteral(string body, bool multiline)
        {
            this.Body = body;
            MultiLine = multiline;
        }

        public string Body
        {
            get;
            private set;
        }

        public bool MultiLine
        {
            get;
            private set;
        }

        public override bool Equals(object obj)
        {
            if ((obj is CommentLiteral) == false)
                return false;
            return this.Body == ((CommentLiteral)obj).Body;
        }

        public override int GetHashCode()
        {
            return this.Body.GetHashCode();
        }

        public override string ToString()
        {
            return this.Body;
        }
    }
}
