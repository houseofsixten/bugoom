# bugoom
Demo API with entity framework migration

The Bugoom API simulates a bug ticketing system, where a user can report bugs, after which staff can assign and fix them.

Open the project in Visual Studio where you can then run the application.

When running the application on your local machine, a sqlite database file will be created in Environment.SpecialFolder.LocalApplicationData, which is [Your Windows drive]:/Users/[Your user name]/AppData/Local. The file name will be "bugging.db".

If you want to reset the database to nothing, you can delete bugging.db in the location specified above.

When running the application for the first time, five users will be created in the Users table: one Boss user, two Staff users, and two User users. On subsequent runs, the application will check the database for the test users and not create them any more if it finds them.
