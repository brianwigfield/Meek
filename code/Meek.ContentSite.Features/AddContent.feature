Feature: Add Content
	In order to mantain my own content
	As a content admin
	I want to be able to add new content to the site

@UXSession
Scenario: I am logged in
	Given I have logged in as a content admin
	When I ask for a non existent page
	Then it should provide me with a blank content editor
	And Allow me to create my content

@UXSession
Scenario: I am not logged in
	When I ask for a non existent page
	Then it should provide me with a not found page