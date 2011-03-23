Feature: Edit Content
	In order to mantain my own content
	As a content admin
	I want to be able to add new content to the site

@UXSession
Scenario: I am logged in
	Given I have logged in as a content admin
	When I ask to edit an existing page
	Then it should provide me with a populated content editor
	And Allow me to edit my existing content

@UXSession
Scenario: I am not logged in
	When I ask to edit an existing page
	Then it should provide me with a not found page