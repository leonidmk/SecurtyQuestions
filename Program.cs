using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurtyQuestions
{
    class Program
    {
        static string strConnectionString = ConfigurationManager.ConnectionStrings["SecurityQuestions"].ConnectionString;
        //Dictionary returned from DB
       static Dictionary<string, string> dictExistingUsersWithAnswers = new Dictionary<string, string>();

        static void Main(string[] args)
        {
            string strInputName = string.Empty;
            string strExistingUser = string.Empty;
            string strStoreAnswers = string.Empty;
            string strAnswer = string.Empty;
            int intCounter = 0;
            bool boolCorrectAnswer = false;
            int intResult=0;

            //Array with questions
            string[] arrQuestions = { "In what city were you born?", "What is the name of your favorite pet?",
                                                  "What is your mother's maiden name?","What high school did you attend?",
                                                  "What was the mascot of your high school?","What was the make of your first car?",
                                                  "What was your favorite toy as a child?","Where did you meet your spouse?",
                                                  "What is your favorite meal?","Who is your favorite actor / actress?",
                                                  "What is your favorite album?"};

            //Dictionary where any of 3 items can be updated with answers
            var dictQuestionsAnswers = new Dictionary<string, string>()
            {
                { "In what city were you born?", string.Empty},
                { "What is the name of your favorite pet?",string.Empty},
                { "What is your mother's maiden name?",string.Empty},
                { "What high school did you attend?",string.Empty},
                { "What was the mascot of your high school?",string.Empty},
                { "What was the make of your first car?", string.Empty},
                { "What was your favorite toy as a child?",string.Empty},
                { "Where did you meet your spouse?",string.Empty},
                { "What is your favorite meal?", string.Empty},
                { "Who is your favorite actor / actress?", string.Empty},
                {  "What is your favorite album?", string.Empty},
            };

            Console.Write("Hi, what is your name?");
            strInputName = Console.ReadLine();

            strInputName = strInputName.Replace(" ", string.Empty);
            strInputName = strInputName.ToLower();

            //check if this user exists
            try
            {
                GetUserByName(strInputName);
            }
            catch
            {
                Console.WriteLine("SQL Server related error.");
            }
           
                if (dictExistingUsersWithAnswers.Count > 0)
                {
                    //If exists, check user's answers for all 3 questions
                    Console.WriteLine($"Please provide answers for the following 3 questions ");
                    Console.WriteLine($"You have 3 tries for each question: <press Enter to continue>");

                    foreach (KeyValuePair<string, string> entry in dictExistingUsersWithAnswers)
                    {
                        if (boolCorrectAnswer == false && (intCounter > 0 && intCounter < 3))
                        {
                            Console.WriteLine("Your answer is incorrect. Try again later");
                            break;
                        }
                        Console.WriteLine(entry.Key);
                        strAnswer = Console.ReadLine().ToLower();

                        if (strAnswer.ToLower() == entry.Value.ToLower())
                        {
                            Console.WriteLine("Your answer is correct: <press Enter to continue>");
                            boolCorrectAnswer = true;
                        }
                        else
                        {
                            for (int i = 0; intCounter < 2; i++)
                            {
                                if (intCounter != 2)
                                {
                                    Console.WriteLine("Your answer is incorrect, try again: <press Enter to continue>");
                                }
                                else
                                {
                                    Console.WriteLine("Your answer is incorrect, you are out of tries: <press Enter to continue>");
                                }
                                strAnswer = Console.ReadLine().ToLower();
                                if (strAnswer.ToLower() == entry.Value.ToLower())
                                {
                                    Console.WriteLine("Your answer is correct: <press Enter to continue>");
                                    boolCorrectAnswer = true;
                                    break;
                                }
                                intCounter = intCounter + 1;
                            }
                        }
                    }
                    if (boolCorrectAnswer == true && (intCounter != 2))
                    {
                        Console.WriteLine("All your 3 answers were successfully verified.");
                    }
                    else
                    {
                        Console.WriteLine("Your answer is incorrect, you are out of tries: <press Enter to finish>");
                    }
                }
                else
                {
                    Console.WriteLine($"Would you like to store answers to security questions?(Y/N):");
                    strStoreAnswers = Console.ReadLine().ToLower();

                    if (strStoreAnswers == "y")
                    {
                        int intEnterKeyPressedQuintity = 0;
                        ConsoleKeyInfo cki;
                        foreach (var item in arrQuestions)
                        {
                            {
                                if (intEnterKeyPressedQuintity == 3)
                                {
                                    break;
                                }
                                Console.WriteLine(" Question: {0} <press Enter to continue>", item);
                                Console.WriteLine(" Your answer: {0}  <press Enter to continue>", dictQuestionsAnswers[item] = Console.ReadLine());
                                if (dictQuestionsAnswers[item] != string.Empty) { intEnterKeyPressedQuintity = intEnterKeyPressedQuintity + 1; }
                            }
                        }

                        //Remove items from dictionary with not answered questions
                        foreach (var s in dictQuestionsAnswers.Where(kv => kv.Value == string.Empty).ToList())
                        {
                            dictQuestionsAnswers.Remove(s.Key);
                        }

                    //Store 3 answered questions in DB
                    try
                    {
                        intResult = InsertUsersAnswer(dictQuestionsAnswers, strInputName);
                    }
                    catch
                    {
                        Console.WriteLine("SQL Server related error.");
                    }
                        if (intResult != 0)
                        {
                            Console.WriteLine(" Your answers were successfully saved!");
                        }
                    }
                    else
                    {
                        //If user does not want to store answers
                        Console.WriteLine(" Sorry. Goodbye!");
                    }
                }
                Console.ReadLine();
            }


        private static void GetUserByName(string User)
        {                      
            string strAnswer = string.Empty;
            string strQuestion = string.Empty;

            try
            {
                using (SqlConnection con = new SqlConnection(strConnectionString))
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("GetAnswersForUser", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = User;

                            con.Open();
                            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                            while (dr.Read())
                            {
                                strAnswer = dr["UserAnswer"].ToString();
                                strQuestion = dr["Question"].ToString();
                                dictExistingUsersWithAnswers.Add(strQuestion, strAnswer);
                            }
                        }
                    }
                    catch (SqlException ex)
                    { throw; }
                }
            }
            catch (SqlException ex)
            {
                throw;
            }
        }

        private static int InsertUsersAnswer(Dictionary<string, string> dictUsersAnswers, string strUser)
        {
            try
            {
                int K = 0;
                using (SqlConnection con = new SqlConnection(strConnectionString))
                {
                    try
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand("AddAnswersForUser", con))
                        {
                            foreach (KeyValuePair<string, string> entry in dictUsersAnswers)
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@Name", strUser);
                                cmd.Parameters.AddWithValue("@Answer", entry.Value);
                                cmd.Parameters.AddWithValue("@Question", entry.Key);
                                K = cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                        }
                    }
                    catch(SqlException ex)
                    {
                        throw;
                    }
                }

                return K;
            }
            catch (SqlException ex)
            {
                throw;
            }
        }
    }
}