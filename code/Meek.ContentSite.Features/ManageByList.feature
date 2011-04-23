Feature: Manage By List
	In order to manage all my sites content
	As a content admin
	I want to be able to view the list of all content

@UXSession
Scenario: I am logged in
	Given I have logged in as a content admin
	When I press navigate to the list page
	Then the it should show a list of links
