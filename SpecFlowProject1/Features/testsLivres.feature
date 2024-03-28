Feature: testsLivres
	Simple calculator for adding two numbers

@recherche 
Scenario: Test recherche par ID
	Given je saisie la valleur "00017" dans le champs de recherche de l'id
	When  je clique sur le bouton de recherche
	Then Le datagridview affiche le livre possédant l'id "00017"
