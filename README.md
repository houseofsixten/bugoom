# bugoom
Demo API with entity framework migration

The Bugoom API simulates a bug ticketing system, where a user can report bugs,
after which staff can assign and fix them.

Open the project in Visual Studio where you can then run the application.

When running the application on your local machine, a sqlite database file will
be created in Environment.SpecialFolder.LocalApplicationData, which is
[Your Windows drive]:/Users/[Your user name]/AppData/Local by default.
The file name will be "bugging.db". If you want to reset the database to nothing,
you can delete bugging.db in the location specified above.

There are three user roles: Boss, Staff, and User.
When running the application for the first time, five users will be created in the Users table:
one Boss user, two Staff users, and two User users. On subsequent runs, the application will
check the database for the test users and not create them any more if it finds them.
Every user has an auto-generated integer ID.

	You can view all the users by using the Users/GetAll endpoint.

There is no endpoint for adding new users, or for logging in as users.
There are no user credentials that are passed to the endpoints.
The API merely simulates user actions by having the endpoints require the IDs.

	There are four bug statuses: Open, Assigned, Fixed, and Closed.

The bug tracking workflow is handled by the BugsController POST endpoints, which are as follows:

	1: Bugs/Open

		Parameters: openedByUserId, title, description.

		Provide the ID of a user with the User or Boss role, a title, and a description.
		A new bug with the title and description will be opened by the given user.
		The new bug will start with the Open status.

	2: Bugs/Assign

		Parameters: bugId, assignedByStaffId, assignedToStaffId, notes

		The bug with the given ID must be in Open or Assigned status.
		assignedByStaffId can be the ID of a Staff or Boss user.
		assignedToStaffId can only be a Staff user.

		The given Staff or Boss will assign the bug to the other given Staff,
		and the bug's status will change to Assigned.

		A Staff may assign the bug to themselves.

	3: Bugs/Fix
		
		Parameters: bugId, fixedByStaffId, notes

		The given Staff must be the staff to whom the bug is assigned.
		The bug's status must be Assigned.

		The bug's status will change to Fixed.

	4: Bugs/Close

		Parameters: bugId, openedByUserId, notes

		The openedByUserId must be the user who opened the bug.
		The bug's status must be Fixed.

		The bug's status will change to Closed.

	5: Bugs/Comment

		Parameters: bugId, staffId, notes

		The staffId must be the ID of a Staff or Boss user.
		The given bug can be in any status.
		The notes will be attached as a comment to the bug.

The Bugs/GetStatus endpoint returns the single bug with the given ID and user. The user has to be the one that opened the bug.
The bug will be displayed with all its changes in oldest to newest order.

The Bugs/GetAll endpoint returns all the bugs by default. It can be filtered by the user that opened the bug,
the user to whom the bug is assigned, or the status of the bug.
