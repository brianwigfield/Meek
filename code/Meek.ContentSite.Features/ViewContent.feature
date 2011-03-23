Feature: View Content
	In order to read the site
	As a user
	I want to be able to view existing content

@UXSession
Scenario: I am content admin
	Given I have logged in as a content admin
	When I navigate to a content page
	Then It should present me with an edit link

@UXSession
Scenario: I am not content admin
	When I navigate to a content page
	Then It should not present me with an edit link

@UXSession
Scenario: Existing partial content should not be directly viewable
    When I navigate to a partial content resource
    Then it should provide me with a not found page