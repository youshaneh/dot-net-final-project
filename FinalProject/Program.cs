using System;
using System.Collections.Generic;
using System.Linq;

namespace FinalProject
{
    class Renderer
    {
        public const String paddingLeft = "   ";
        List<Component> components;

        public Renderer clear()
        {
            Console.Clear();
            components = new List<Component>();
            return this;
        }

        public Renderer addComponent(Component component)
        {
            components.Add(component);
            return this;
        }

        public void render()
        {
            Console.SetCursorPosition(0, 0);
            components.ForEach(component => {
                foreach(String componentLine in component.getRenderedLines())
                {
                    String line = paddingLeft + componentLine;
                    if (component.getClearTheRestOfEachLine())
                    {
                        Console.Write("{0}{1}", line, new string(' ', Console.WindowWidth - line.Length % Console.WindowWidth));
                    }
                    else
                    {
                        Console.WriteLine("{0}", line);
                    }
                }
            });
        }
    }

    interface Component
    {
        bool getClearTheRestOfEachLine();
        String[] getRenderedLines();
    }

    class MessageComponent : Component
    {
        private String[] messages;
        private bool clearTheRestOfEachLine = true;

        public MessageComponent() { }
        public MessageComponent(params String[] messages)
        {
            setMessages(messages);
        }
        public void setMessages(params String[] messages)
        {
            this.messages = messages;
        }
        public void setClearTheRestOfEachLine(bool clear)
        {
            clearTheRestOfEachLine = clear;
        }
        public String[] getRenderedLines()
        {
            return messages;
        }
        public bool getClearTheRestOfEachLine()
        {
            return clearTheRestOfEachLine;
        }
    }

    class MultipleChoiceComponent : Component
    {
        private String[] choices;
        private int selectedIndex;

        public void setSelectedIndex(int index)
        {
            if(index < 0)
            {
                selectedIndex = 0;
                return;
            }
            if (index >= choices.Length)
            {
                selectedIndex = choices.Length - 1;
                return;
            }
            selectedIndex = index;
        }

        public int getSelectedIndex() { return selectedIndex; }

        public MultipleChoiceComponent(params String[] choices)
        {
            if (choices.Length == 0) throw new Exception("At least one option should be provided");
            this.choices = choices;
        }

        public String[] getRenderedLines()
        {
            String[] lines = new String[choices.Length];
            for (int i = 0; i < choices.Length; i++)
            {
                lines[i] = ""+ ((i == selectedIndex)? "->": "  ") + choices[i];
            }
            return lines;
        }

        public bool getClearTheRestOfEachLine()
        {
            return true;
        }
    }

    class PasswordComponent : Component
    {
        private String password;
        private List<String> messages;

        public PasswordComponent(String password)
        {
            this.password = password;
        }

        public void setMessages(params String[] messages)
        {
            this.messages = messages.ToList();
        }

        public String[] getRenderedLines()
        {
            List<String> lines = messages.GetRange(0, messages.Count);
            lines.Add("");
            lines.Add("Please Enter the Admin Password:");
            return lines.ToArray();
        }

        public bool getClearTheRestOfEachLine()
        {
            return true;
        }
    }

    class Movie
    {
        private String name;
        private String ageLimit;

        public String getName()
        {
            return name;
        }
        public String getAgeLimit()
        {
            return ageLimit;
        }

        public Movie(String name, String ageLimit)
        {
            this.name = name;
            this.ageLimit = ageLimit;
        }
    }

    public delegate Page Page();

    class Program
    {
        const String PASSWORD = "1234";

        static Renderer renderer = new Renderer();
        static MessageComponent bannderComponent = new MessageComponent(
                "",
                "███╗   ███╗ ██████╗ ██╗   ██╗██╗███████╗██████╗ ██╗     ███████╗██╗  ██╗",
                "████╗ ████║██╔═══██╗██║   ██║██║██╔════╝██╔══██╗██║     ██╔════╝╚██╗██╔╝",
                "██╔████╔██║██║   ██║██║   ██║██║█████╗  ██████╔╝██║     █████╗   ╚███╔╝ ",
                "██║╚██╔╝██║██║   ██║╚██╗ ██╔╝██║██╔══╝  ██╔═══╝ ██║     ██╔══╝   ██╔██╗ ",
                "██║ ╚═╝ ██║╚██████╔╝ ╚████╔╝ ██║███████╗██║     ███████╗███████╗██╔╝ ██╗",
                "╚═╝     ╚═╝ ╚═════╝   ╚═══╝  ╚═╝╚══════╝╚═╝     ╚══════╝╚══════╝╚═╝  ╚═╝",
                "",
                "");

        static Movie[] movies = new Movie[0];

        static void Main(string[] args)
        {
            Page nextPage = new Page(home);
            while(nextPage != null)
            {
                try
                {
                    nextPage = nextPage();
                }
                catch
                {
                    Console.Clear();
                    Console.Write("\n{0}Something went wrong. You will be redirected to the main page after pressing any key...", Renderer.paddingLeft);
                    Console.ReadKey();
                    nextPage = new Page(home);
                }
            }
        }

        static Page home()
        {
            Console.CursorVisible = false;
            MultipleChoiceComponent mainMenuComponent = new MultipleChoiceComponent("Administrator", "Guests");
            MessageComponent instructionComponent = new MessageComponent("Please select from the following options:", "");
            renderer
                .clear()
                .addComponent(bannderComponent)
                .addComponent(instructionComponent)
                .addComponent(mainMenuComponent);
            while (true)
            {
                renderer.render();
                ConsoleKey key = Console.ReadKey().Key;
                clearPreviousLines(0);
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        {
                            mainMenuComponent.setSelectedIndex(mainMenuComponent.getSelectedIndex() - 1);
                            break;
                        }
                    case ConsoleKey.DownArrow:
                        {
                            mainMenuComponent.setSelectedIndex(mainMenuComponent.getSelectedIndex() + 1);
                            break;
                        }
                    case ConsoleKey.Enter:
                        {
                            return (mainMenuComponent.getSelectedIndex() == 0)? new Page(password) : new Page(guest);
                        }
                }
            }
        }
        static Page password()
        {
            Console.CursorVisible = true;
            const String BACK = "B";
            int attemptLeft = 5;

            PasswordComponent passwordComponent = new PasswordComponent(PASSWORD);
            passwordComponent.setMessages("", "");
            renderer
                .clear()
                .addComponent(bannderComponent)
                .addComponent(passwordComponent);
            while (true)
            {
                renderer.render();
                Console.Write(Renderer.paddingLeft);
                String input = Console.ReadLine();
                switch (input)
                {
                    case PASSWORD:
                        {
                            return new Page(administrator);
                        }
                    case BACK:
                        {
                            return new Page(home);
                        }
                    default:
                        {
                            attemptLeft--;
                            if(attemptLeft <= 0) return new Page(home);
                            Console.Clear();
                            passwordComponent.setMessages(
                                "You entered an Invalid password",
                                "You have "+ attemptLeft + " more attempts to enter the correct password OR Press "+ BACK + " to go back to the previous screen.");
                            break;
                        }
                }
            }
        }
        static Page administrator()
        {
            Console.CursorVisible = true;
            const int MAX_MOVIE_COUNT = 10;
            String[] N_TH_STRING = new String[]{ "First", "Second", "Third", "Fourth", "Fifth", "Sixth", "Seventh", "Eighth", "Nineth", "Tenth" };

            Console.Clear();
            Console.WriteLine("\n{0}Welecome MobiePlex Administrator\n", Renderer.paddingLeft);

            int movieCount = -1;
            while(true)
            {
                Console.Write("{0}How many movies are playing today?: ", Renderer.paddingLeft);
                String movieCountInput = Console.ReadLine();
                if(int.TryParse(movieCountInput, out movieCount))
                {
                    if (movieCount >= 0 && movieCount <= MAX_MOVIE_COUNT)
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("{0}The number should be between 0 and 10. Please try again.", Renderer.paddingLeft);
                    }
                }
                else
                {
                    Console.WriteLine("{0}Invalid input. Please try again.", Renderer.paddingLeft);
                }
            }
            Console.WriteLine("");
            Console.WriteLine("   G – General Audience, any age is good");
            Console.WriteLine("   PG – We will take PG as 10 years or older");
            Console.WriteLine("   PG - 13 – We will take PG-13 as 13 years or older");
            Console.WriteLine("   R – We will take R as 15 years or older.Don’t worry about accompany by parent case.");
            Console.WriteLine("   NC - 17 – We will take NC-17 as 17 years or older");
            Program.movies = new Movie[movieCount];
            for(int i = 0; i < movieCount; i++)
            {
                Console.Write("\n{0}Please Enter the {1} Movie's Name: ", Renderer.paddingLeft, N_TH_STRING[i]);
                String name = Console.ReadLine();
                while(name.Trim(' ') == "")
                {
                    Console.WriteLine("{0}Movie name can not be empty. Please try again.", Renderer.paddingLeft);
                    Console.Write("{0}Please Enter the {1} Movie's Name: ", Renderer.paddingLeft, N_TH_STRING[i]);
                    name = Console.ReadLine();
                }
                Console.Write("{0}Please Enter the Age Limit or Rating for the {1} Movie: ", Renderer.paddingLeft, N_TH_STRING[i]);
                String ageLimitOrRating = Console.ReadLine();
                while (getMinimumAge(ageLimitOrRating) < 0)
                {
                    Console.WriteLine("{0}Invalid input. Allowed values are G, PG, PG-13, R, NC-17, or a positive integer. Please try again.", Renderer.paddingLeft);
                    Console.Write("{0}Please Enter the Age Limit or Rating for the {1} Movie: ", Renderer.paddingLeft, N_TH_STRING[i]);
                    ageLimitOrRating = Console.ReadLine();
                }
                movies[i] = new Movie(name, ageLimitOrRating);
            }

            String[] movieListItems = new string[movies.Length];
            for (int i = 0; i < movies.Length; i++)
            {
                movieListItems[i] = Renderer.paddingLeft + (i + 1) + ". " + movies[i].getName() + "{" + movies[i].getAgeLimit() + "}";
            }
            MessageComponent movieListComponent = new MessageComponent(movieListItems);

            renderer
                .clear()
                .addComponent(bannderComponent)
                .addComponent(movieListComponent)
                .addComponent(new MessageComponent("", "Your Movies Playing Today Are Listed Above. Are you satisfied? (Y/N)"));

            while (true)
            {
                Console.Clear();
                renderer.render();
                Console.Write(Renderer.paddingLeft);
                String confirmed = Console.ReadLine();
                switch (confirmed)
                {
                    case "Y":
                    case "y":
                        {
                            return new Page(home);
                        }
                    case "N":
                    case "n":
                        {
                            return new Page(administrator);
                        }
                    default:
                        {
                            Console.Write("{0}Invalid input. Press any key to continue.", Renderer.paddingLeft);
                            Console.ReadKey();
                            break;
                        }
                }
            }
        }
        static Page guest()
        {
            Console.CursorVisible = false;
            if (Program.movies.Length == 0)
            {
                renderer
                    .clear()
                    .addComponent(bannderComponent)
                    .addComponent(new MessageComponent("No moive available. Press any key to go back to the Start Page."));
                renderer.render();
                Console.ReadKey();
                return new Page(home);
            }

            MessageComponent movieMenuTitleComponemt = new MessageComponent(
                "There are " + Program.movies.Length + " movies playing today. Please choose from the following movies:",
                "Which Movie Would You Like to Watch:"
            );
            String[] options = new String[Program.movies.Length];
            for (int i = 0; i < Program.movies.Length; i++)
            {
                options[i] = (i + 1) + ". " + Program.movies[i].getName() + "{" + Program.movies[i].getAgeLimit() + "}";
            }
            MultipleChoiceComponent movieMenuComponent = new MultipleChoiceComponent(options);
            renderer
                .clear()
                .addComponent(bannderComponent)
                .addComponent(movieMenuTitleComponemt)
                .addComponent(movieMenuComponent);

            int selectedIndex = -1;
            while (selectedIndex < 0)
            {
                renderer.render();
                ConsoleKey key = Console.ReadKey().Key;
                clearPreviousLines(0);
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        {
                            movieMenuComponent.setSelectedIndex(movieMenuComponent.getSelectedIndex() - 1);
                            break;
                        }
                    case ConsoleKey.DownArrow:
                        {
                            movieMenuComponent.setSelectedIndex(movieMenuComponent.getSelectedIndex() + 1);
                            break;
                        }
                    case ConsoleKey.Enter:
                        {
                            selectedIndex = movieMenuComponent.getSelectedIndex();
                            break;
                        }
                }
            }
            Console.WriteLine("");
            Console.Write(Renderer.paddingLeft);
            Console.Write("Please Enter Your Age for Verification: ");
            String ageInput = Console.ReadLine();
            if (int.TryParse(ageInput, out int userAge))
            {
                int minimumAgeForTheMovie = getMinimumAge(Program.movies[selectedIndex].getAgeLimit());
                if (userAge < minimumAgeForTheMovie)
                {
                    Console.WriteLine("{0}You are under the age limit. Press any key to choose another movie.", Renderer.paddingLeft);
                    Console.ReadKey();
                    return new Page(guest);
                }
            }
            else
            {
                Console.WriteLine("{0}Invalid input. Press any key to start over.", Renderer.paddingLeft);
                Console.ReadKey();
                return new Page(guest);
            }

            MessageComponent ageInputPlaceholderComponent = new MessageComponent("", "");
            ageInputPlaceholderComponent.setClearTheRestOfEachLine(false);
            MessageComponent enjoyTheMovieComponent = new MessageComponent("", "Enjoy the Movie!", "");
            MultipleChoiceComponent goBackPageMenuComponent = new MultipleChoiceComponent("go back to the Guest Main Menu.", "go back to the Start Page.");
            renderer
                .addComponent(ageInputPlaceholderComponent)
                .addComponent(enjoyTheMovieComponent)
                .addComponent(goBackPageMenuComponent);
            while (true)
            {
                renderer.render();
                ConsoleKey key = Console.ReadKey().Key;
                clearPreviousLines(0);
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        {
                            goBackPageMenuComponent.setSelectedIndex(goBackPageMenuComponent.getSelectedIndex() - 1);
                            break;
                        }
                    case ConsoleKey.DownArrow:
                        {
                            goBackPageMenuComponent.setSelectedIndex(goBackPageMenuComponent.getSelectedIndex() + 1);
                            break;
                        }
                    case ConsoleKey.Enter:
                        {
                            return (goBackPageMenuComponent.getSelectedIndex() == 0) ? new Page(guest) : new Page(home);
                        }
                }
            }

        }

        private static int getMinimumAge(String ageLimitOrRating)
        {
            if (int.TryParse(ageLimitOrRating, out int ageLimit))
            {
                return ageLimit;
            }
            switch (ageLimitOrRating)
            {
                case "G": return 0;
                case "PG": return 10;
                case "PG-13": return 13;
                case "R": return 15;
                case "NC-17": return 17;
            }
            return -1;
        }

        private static void clearPreviousLines(int lineCount)
        {
            Console.SetCursorPosition(0, Console.CursorTop - lineCount);
            for(int i = 0; i <= lineCount; i++)
            {
                Console.Write(new string(' ', Console.WindowWidth));
            }
            Console.SetCursorPosition(0, Console.CursorTop - lineCount - 1);
        }
    }
}
