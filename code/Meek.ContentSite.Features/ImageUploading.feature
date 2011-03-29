Feature: Image Uploading
	In order to add images to content
	As a content admin
	I want to be able to upload images to the server

@UXSession
Scenario: I am logged in
	Given I have logged in as a content admin
	When I ask to edit an existing page
	And I choose a file & press upload
	Then it should save it to the server