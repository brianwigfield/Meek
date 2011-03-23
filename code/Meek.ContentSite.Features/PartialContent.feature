Feature: Partial Content
	In order to maintain my own content
	As a content admin
	I want to be able to maintain partial page content

@UXSession
Scenario: Edit content
	Given I have logged in as a content admin
	When I ask to edit an existing partial content
	Then it should provide me with a populated partial content editor
	And allow me to edit my existing partial content
    And return me to the home page
    And let me navigate to view my partial content

@UXSession
Scenario: Viewing content logged in
	Given I have logged in as a content admin
    When I ask to view an existing page with partial content
    Then It should present me with an edit link

@UXSession
Scenario: Viewing content not logged in
    When I ask to view an existing page with partial content
    Then It should not present me with an edit link

@UXSession
Scenario: Viewing non existent partial content logged in
	Given I have logged in as a content admin
    When I ask for a page with non existent partial content
    Then it should present me with a create content link

@UXSession
Scenario: Viewing non existent partial content not logged in
    When I ask for a page with non existent partial content
    Then it should show an empty section