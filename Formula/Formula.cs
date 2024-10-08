﻿// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta)
// Version 1.2 (9/10/17)

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens
using SpreadsheetUtilities;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// Author:    Shu Chen
/// Partner:   None
/// Date:      2024/2/6
/// Course:    CS 3500, University of Utah, School of Computing
/// Copyright: CS 3500 and Shu Chen - This work may not
///            be copied for use in Academic Coursework.
///
/// I, Shu Chen, certify that I wrote this code from scratch and
/// did not copy it in part or whole from another source.  All
/// references used in the completion of the assignments are cited
/// in my README file.
///
/// File Contents
///
/// This is a class to eval a formula by calling evaluator class
/// </summary>
namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision
    /// floating-point syntax (without unary preceeding '-' or '+');
    /// variables that consist of a letter or underscore followed by
    /// zero or more letters, underscores, or digits; parentheses; and the four operator
    /// symbols +, -, *, and /.
    ///
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable;
    /// and "x 23" consists of a variable "x" and a number "23".
    ///
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        /// <summary>
        /// This is a Read only collection to store tokens of the formula
        /// </summary>
        private ReadOnlyCollection<string> validFormula;

        /// <summary>
        /// To track the unique variables in the formula
        /// </summary>
        private HashSet<string> variables;

        /// <summary>
        /// The final formula in a string form
        /// </summary>
        private string finalFormula;

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        ///
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        ///
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.
        ///
        /// If the formula contains a variable v such that normalize(v) is not a legal variable,
        /// throws a FormulaFormatException with an explanatory message.
        ///
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        ///
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        ///
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            variables = new HashSet<string>();

            IEnumerable<string> formulaTokens = GetTokens(formula);
            if (formulaTokens.Count() < 1) { throw new FormulaFormatException("Formula need has at least one token! - Rule 2"); }
            List<string> vaildTokens = CheckTokenValid.CreateAValidTokenList(formulaTokens, normalize, isValid, out variables);
            validFormula = new ReadOnlyCollection<string>(vaildTokens);
            InitializeToString();
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to
        /// the constructor.)
        ///
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters
        /// in a string to upper case:
        ///
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        ///
        /// Given a variable symbol as its parameter, lookup returns the variable's value
        /// (if it has one) or throws an ArgumentException (otherwise).
        ///
        /// If no undefined variables or divisions by zero are encountered when evaluating
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            return Evaluator.Evaluate(validFormula, lookup);
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this
        /// formula.  No normalization may appear more than once in the enumeration, even
        /// if it appears more than once in this Formula.
        ///
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            return variables;
        }

        /// <summary>
        /// A helper method to make the formula in to string, and ToString method just need to get the final formula variable.
        /// </summary>
        private void InitializeToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string tokens in validFormula)
            {
                sb.Append(tokens);
            }
            this.finalFormula = sb.ToString();
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        ///
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            return this.finalFormula;
        }

        /// <summary>
        ///  <change> make object nullable </change>
        ///
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        ///
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized"
        /// by C#'s standard conversion from string to double, then back to string. This
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as
        /// defined by the provided normalizer.
        ///
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (!(obj is Formula)) return false;
            ReadOnlyCollection<string> formula2 = ((Formula)obj).GetFormula();
            if (validFormula.Count != formula2.Count) return false;
            for (int i = 0; i < validFormula.Count; i++)
            {
                if (!validFormula[i].Equals(formula2[i])) return false;
            }
            return true;
        }

        /// <summary>
        ///   <change> We are now using Non-Nullable objects.  Thus neither f1 nor f2 can be null!</change>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        ///
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            return f1.Equals(f2);
        }

        /// <summary>
        ///   <change> We are now using Non-Nullable objects.  Thus neither f1 nor f2 can be null!</change>
        ///   <change> Note: != should almost always be not ==, if you get my meaning </change>
        ///   Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return !f1.Equals(f2);
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*//*]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }
        }

        /// <summary>
        /// return the formula
        /// </summary>
        /// <returns></returns>
        public ReadOnlyCollection<string> GetFormula()
        { return validFormula; }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}

/// <summary>
/// Check token if it is valid or not
/// </summary>
internal class CheckTokenValid
{
    /// <summary>
    /// Check every tokens in the input tokens is valid or not, if not throw error, if yes put it in to list
    /// Finished Rule: 1 2 3 4 5 6 7 8
    /// </summary>
    /// <param name="inputTokens">Tokens needs to be check</param>
    /// <param name="normalize">Normalize Func</param>
    /// <param name="isValid">is Valid Func</param>
    /// <returns>a list of valid token</returns>
    /// <exception cref="FormulaFormatException">Throw exception if the token has problem</exception>
    public static List<string> CreateAValidTokenList(IEnumerable<string> inputTokens,
                                                    Func<string, string> normalize,
                                                    Func<string, bool> isValid,
                                                    out HashSet<string> variables)
    {
        List<string> inputTokenList = inputTokens.ToList();
        List<string> vaildTokens = new List<string>();
        variables = new HashSet<string>();
        bool isFirstToken = true;
        int leftParent = 0;
        int rightParent = 0;

        for (int i = 0; i < inputTokenList.Count; i++)
        {
            string token = inputTokenList[i];
            double value = 0;

            //Add it if it is a number
            if (CheckTokenValid.IsNumber(token, out value))
            {
                if (!IsNextTokenOPorRP(inputTokenList, i)) throw new FormulaFormatException("Any token that immediately follows a number, a variable, or a closing parenthesis must be either an operator or a closing parenthesis. - Rule 8");
                isFirstToken = false;
                vaildTokens.Add(value.ToString());
                continue;
            }

            //Add it if it is an operator
            if (CheckTokenValid.IsOperator(token))
            {
                if (token.Equals("("))
                {
                    leftParent++;
                    if (!IsNextTokenNumberOrLP(inputTokenList, i)) throw new FormulaFormatException("Any token that immediately follows an opening parenthesis or an operator must be either a number, a variable, or an opening parenthesis! - Rule 7");
                }
                else if (token.Equals(")"))
                {
                    rightParent++;
                    if (!IsNextTokenOPorRP(inputTokenList, i)) throw new FormulaFormatException("Any token that immediately follows a number, a variable, or a closing parenthesis must be either an operator or a closing parenthesis. - Rule 8");
                    if (rightParent > leftParent) throw new FormulaFormatException("Right Parentheses is more than Left Parentheses! - Rule 3");
                }
                else if (Regex.IsMatch(token, @"[\+\-*//*]"))
                {
                    if (isFirstToken) { throw new FormulaFormatException($"Can not start the expression with {token}! - Rule 5"); }
                    if (!IsNextTokenNumberOrLP(inputTokenList, i)) throw new FormulaFormatException("Any token that immediately follows an opening parenthesis or an operator must be either a number, a variable, or an opening parenthesis! - Rule 7");
                }

                isFirstToken = false;
                vaildTokens.Add(token);
                continue;
            }

            //Add it if this variable is valid.
            if (CheckTokenValid.IsVariable(token))
            {
                string newToken = normalize(token);
                if (isValid(newToken))
                {
                    isFirstToken = false;
                    vaildTokens.Add(newToken);
                    variables.Add(newToken);
                    if (!IsNextTokenOPorRP(inputTokenList, i)) throw new FormulaFormatException("Any token that immediately follows a number, a variable, or a closing parenthesis must be either an operator or a closing parenthesis. - Rule 8");
                    continue;
                }
                else throw new FormulaFormatException($"The token {token} is not valid after converted to {newToken}!");
            }
            else throw new FormulaFormatException($"The token {token} is a not valid operator or number! - Rule 1");
        }

        //can't reach this Exception
        if (Regex.IsMatch(vaildTokens[vaildTokens.Count - 1], @"[\(\+\-*//*]"))
            throw new FormulaFormatException("The Last Token must be a number, variable or )! - Rule 6");
        if (leftParent != rightParent) throw new FormulaFormatException("Number of left and right parenthesis is not equal! - Rule 4");
        return vaildTokens;
    }

    /// <summary>
    /// Return true if next token is a number || variable || LeftParenthesis
    /// </summary>
    /// <returns></returns>
    private static bool IsNextTokenNumberOrLP(List<string> tokens, int index)
    {
        if (index < tokens.Count() - 1)
        {
            string nextToken = tokens[index + 1];
            if (CheckTokenValid.IsVariable(nextToken) ||
                CheckTokenValid.IsNumber(nextToken, out double foo) ||
                nextToken.Equals("("))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Return true if next token is an Operator or Right Parenthesis
    /// </summary>
    /// <returns></returns>
    private static bool IsNextTokenOPorRP(List<string> tokens, int index)
    {
        if (index < tokens.Count() - 1)
        {
            string nextToken = tokens[index + 1];
            if (CheckTokenValid.IsOperator(nextToken) ||
                nextToken.Equals(")"))
            {
                return true;
            }
            return false;
        }
        return true;
    }

    /// <summary>
    /// Check if this token is a number
    /// </summary>
    /// <param name="token"></param>
    /// <param name="number"></param>
    /// <returns></returns>
    private static bool IsNumber(string token, out double number)
    { return double.TryParse(token, out number); }

    /// <summary>
    /// Check if this token is a variable
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public static bool IsVariable(string token)
    { return Regex.IsMatch(token, @"[a-zA-Z_](?: [a-zA-Z_]|\d)*"); }

    /// <summary>
    /// Check if this token is an operator
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    private static bool IsOperator(string token)
    { return Regex.IsMatch(token, @"[\(\)\+\-*//*]"); }
}

// <change>
//   If you are using Extension methods to deal with common stack operations (e.g., checking for
//   an empty stack before peeking) you will find that the Non-Nullable checking is "biting" you.
//
//   To fix this, you have to use a little special syntax like the following:
//
//       public static bool OnTop<T>(this Stack<T> stack, T element1, T element2) where T : notnull
//
//   Notice that the "where T : notnull" tells the compiler that the Stack can contain any object
//   as long as it doesn't allow nulls!
// </change>