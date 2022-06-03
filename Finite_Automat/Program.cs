using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineINfix
{
    public enum OperatorType { MULTIPLY, DIVIDE, ADD, SUBTRACT, EXPONENTIAL, OPAREN, CPAREN, VARIABLE, SIN, COS, TG, CTG, LN, LOG, LD, EXP, TOCKA, E, EMALO };
    public interface Element
    {
    }

    public class NumberElement : Element
    {
        double number;
        public Double getNumber()
        {
            return number;
        }

        public NumberElement(String number)
        {
            this.number = Double.Parse(number);
        }

        public override String ToString()
        {
            return ((int)number).ToString();
        }
    }

    public class OperatorElement : Element
    {
        public OperatorType type;
        char c;
        int brojac = 0;
        public OperatorElement(char op)
        {


            c = op;
            if (op == '+')
                type = OperatorType.ADD;
            else if (op == '-')
                type = OperatorType.SUBTRACT;
            else if (op == '*')
                type = OperatorType.MULTIPLY;
            else if (op == '/')
                type = OperatorType.DIVIDE;
            else if (op == '^')
                type = OperatorType.EXPONENTIAL;
            else if (op == 'e')
                type = OperatorType.EMALO;
            else if (op == 'E')
                type = OperatorType.E;
            else if (op == '(')
                type = OperatorType.OPAREN;
            else if (op == ')')
                type = OperatorType.CPAREN;
            else if (op == 'x' || op == 'X')
                type = OperatorType.VARIABLE;
            else if (op == 'n')
                type = OperatorType.SIN;
            else if (op == 's')
                type = OperatorType.COS;
            else if (op == '.')
                type = OperatorType.TOCKA;
            else if (op == 'c')
            {
                brojac = 1;
            }

            else if (op == 'g')
            {
                if (brojac == 1)
                    type = OperatorType.CTG;

                if (brojac == 0)
                    type = OperatorType.TG;
            }


        }

        public override String ToString()
        {
            return c.ToString();
        }
    }

    public class Parser
    {
        List<Element> e = new List<Element>();
        public List<Element> Parse(String s)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (Char.IsDigit(c))
                    sb.Append(c);
                if (i + 1 < s.Length)
                {
                    char d = s[i + 1];
                    if (Char.IsDigit(d) == false && sb.Length > 0)
                    {
                        e.Add(new NumberElement(sb.ToString()));
                        //clears stringbuilder
                        sb.Remove(0, sb.Length);
                    }
                }

                if (c == '+' || c == '-' || c == '*' || c == '/' || c == '^'
                        || c == '(' || c == ')' || c == 'e' || c == 'E' || c == 'n' || c == '.' || c == 's' || c == 'g')
                    e.Add(new OperatorElement(c));
            }
            if (sb.Length > 0)
                e.Add(new NumberElement(sb.ToString()));

            return e;
        }
    }


    public class InfixToPostfix
    {
        List<Element> converted = new List<Element>();
        int Precedence(OperatorElement c)
        {
            if (c.type == OperatorType.E || c.type == OperatorType.EXPONENTIAL || c.type == OperatorType.TOCKA || c.type == OperatorType.SIN || c.type == OperatorType.COS || c.type == OperatorType.TG || c.type == OperatorType.CTG)
                return 2;
            else if (c.type == OperatorType.EMALO || c.type == OperatorType.MULTIPLY || c.type == OperatorType.DIVIDE)
                return 3;
            else if (c.type == OperatorType.ADD || c.type == OperatorType.SUBTRACT)
                return 4;
            else if (c.type == OperatorType.VARIABLE)
                return 5;



            else
                return Int32.MaxValue;
        }

        public void ProcessOperators(Stack<Element> st, Element element, Element top)
        {
            while (st.Count > 0 && Precedence((OperatorElement)element) >= Precedence((OperatorElement)top))
            {
                Element p = st.Pop();
                if (((OperatorElement)p).type == OperatorType.OPAREN)
                    break;
                converted.Add(p);
                if (st.Count > 0)
                    top = st.First();
            }
        }
        public List<Element> ConvertFromInfixToPostFix(List<Element> e)
        {
            List<Element> stack1 = new List<Element>(e);
            Stack<Element> st = new Stack<Element>();
            for (int i = 0; i < stack1.Count; i++)
            {
                Element element = stack1[i];
                if (element.GetType().Equals(typeof(OperatorElement)))
                {
                    if (st.Count == 0 ||
                            ((OperatorElement)element).type == OperatorType.OPAREN)
                        st.Push(element);
                    else
                    {
                        Element top = st.First();
                        if (((OperatorElement)element).type == OperatorType.CPAREN)
                            ProcessOperators(st, element, top);
                        else if (Precedence((OperatorElement)element) < Precedence((OperatorElement)top))
                            st.Push(element);
                        else
                        {
                            ProcessOperators(st, element, top);
                            st.Push(element);
                        }
                    }
                }
                else
                    converted.Add(element);
            }

            //pop all operators in stack
            while (st.Count > 0)
            {
                Element b1 = st.Pop();
                converted.Add(b1);
            }

            return converted;
        }

        public override String ToString()
        {
            StringBuilder s = new StringBuilder();
            for (int j = 0; j < converted.Count; j++)
                s.Append(converted[j].ToString() + " ");
            return s.ToString();
        }
    }

    public class PostFixEvaluator
    {
        Stack<Element> stack = new Stack<Element>();

        NumberElement calculate(NumberElement left, NumberElement right, OperatorElement op)
        {
            Double temp = Double.MaxValue;
            if (op.type == OperatorType.ADD)
                temp = left.getNumber() + right.getNumber();
            else if (op.type == OperatorType.SUBTRACT)
            {
                if (left.getNumber() == null)
                {
                    temp = -1 * right.getNumber();
                }
                else
                {
                    temp = left.getNumber() - right.getNumber();
                }

            }

            else if (op.type == OperatorType.MULTIPLY)
                temp = left.getNumber() * right.getNumber();
            else if (op.type == OperatorType.DIVIDE)
                temp = left.getNumber() / right.getNumber();
            else if (op.type == OperatorType.EXPONENTIAL)
                temp = Math.Pow(left.getNumber(), right.getNumber());
            else if (op.type == OperatorType.VARIABLE)
                temp = right.getNumber();
            else if (op.type == OperatorType.EMALO)
                temp = left.getNumber() * Math.E;
            else if (op.type == OperatorType.E)
                temp = left.getNumber() * Math.Pow(10, right.getNumber());
            else if (op.type == OperatorType.TOCKA)
            {
                if (right.getNumber() < 10 && right.getNumber() > 0)
                {
                    temp = right.getNumber() / 10 + left.getNumber();
                }
                if (right.getNumber() < 100 && right.getNumber() >= 10)
                {
                    temp = right.getNumber() / 100 + left.getNumber();
                }
                if (right.getNumber() < 1000 && right.getNumber() >= 100)
                {
                    temp = right.getNumber() / 1000 + left.getNumber();
                }
                if (right.getNumber() < 10000 && right.getNumber() >= 1000)
                {
                    temp = right.getNumber() / 10000 + left.getNumber();
                }
            }
            else if (op.type == OperatorType.SIN)
                temp = Math.Sin(right.getNumber());
            else if (op.type == OperatorType.COS)
                temp = Math.Cos(right.getNumber());
            else if (op.type == OperatorType.CTG)
            {
                temp = Math.Atan(right.getNumber());
            }
            else if (op.type == OperatorType.TG)
            {
                temp = Math.Tan(right.getNumber());
            }

            return new NumberElement(temp.ToString());
        }
        public Double Evaluate(List<Element> e)
        {
            List<Element> v = new List<Element>(e);
            for (int i = 0; i < v.Count; i++)
            {
                Element element = v[i];
                if (element.GetType().Equals(typeof(NumberElement)))
                    stack.Push(element);
                if (element.GetType().Equals(typeof(OperatorElement)))
                {
                    NumberElement right = (NumberElement)stack.Pop();
                    NumberElement left = (NumberElement)stack.Pop();
                    NumberElement result = calculate(left, right, (OperatorElement)element);
                    stack.Push(result);
                }
            }
            return ((NumberElement)stack.Pop()).getNumber();
        }
    }

    class Program
    {
        public double Calculate(String s)
        {
            Parser p = new Parser();
            List<Element> e = p.Parse(s);
            InfixToPostfix i = new InfixToPostfix();
            e = i.ConvertFromInfixToPostFix(e);

            PostFixEvaluator pfe = new PostFixEvaluator();
            return pfe.Evaluate(e);
        }
        internal static int Prec(char ch)
        {
            switch (ch)
            {
                case '+':
                case '-':
                    return 1;

                case '*':
                case '/':
                    return 2;

                case 'E':
                    return 3;

                case 'e':
                    return 3;

            }
            return -1;
        }

        public static string infixToPostfix(string exp)
        {
            // initializing empty String for result
            string result = "";

            // initializing empty stack
            Stack<char> stack = new Stack<char>();

            for (int i = 0; i < exp.Length; ++i)
            {
                char c = exp[i];

                // If the scanned character is an
                // operand, add it to output.
                if (char.IsLetterOrDigit(c))
                {

                    result += c;
                }

                // If the scanned character is an '(',
                // push it to the stack.
                else if (c == '(')
                {
                    stack.Push(c);
                }

                //  If the scanned character is an ')',
                // pop and output from the stack 
                // until an '(' is encountered.
                else if (c == ')')
                {
                    while (stack.Count > 0 &&
                            stack.Peek() != '(')
                    {
                        result += stack.Pop();
                    }

                    if (stack.Count > 0 && stack.Peek() != '(')
                    {
                        return "Invalid Expression"; // invalid expression
                    }
                    else
                    {
                        stack.Pop();
                    }
                }
                else // an operator is encountered
                {
                    while (stack.Count > 0 && Prec(c) <=
                                      Prec(stack.Peek()))
                    {
                        result += stack.Pop();
                    }
                    stack.Push(c);
                }

            }

            // pop all the operators from the stack
            while (stack.Count > 0)
            {
                result += stack.Pop();
            }

            return result;
        }
        static void Main(string[] args)
        {
            string expa = "";
            string exp = Console.ReadLine();
            if (exp.EndsWith("e") || exp.EndsWith("+") || exp.EndsWith("-") || exp.EndsWith("E"))
            {
                expa = "0" + exp + "0";
                expa = expa.Replace("e", "e0");
            }
            else
            {
                expa = "0" + exp;
                expa = expa.Replace("e", "e0");
            }



            Console.WriteLine(infixToPostfix(exp));
            Program c = new Program();
            double d = c.Calculate(expa);
            Console.WriteLine(d);
            Console.ReadKey();

        }
    }
}
