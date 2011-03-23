Feature: Delete Content
	In order to remove content from the site
	As a content admin
	I want to be able to delete from the edit content page

@UXSession
Scenario: I am logged in
	Given I have logged in as a content admin
	When I ask to edit an un-needed page
	Then it should provide me with a populated content editor
	And allow me to delete the content and return me to the home page