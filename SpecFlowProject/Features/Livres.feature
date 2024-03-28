Feature: testsLivres

Scenario: Test recherche id
	Given je saisie la valleur dans le champs de recherche de l'id : "00024"
	When  je clique sur le bouton de recherche
	Then  la vue affiche le livre possédant l'id : "00024"

Scenario: Test recherche titre
	Given je saisie la valleur dans le champs de recherche des titres: "La planète des singes"
	Then  la vue affiche le livre possédant le titre  : "La planète des singes"

Scenario: Test rayon
	Given je selectionne le rayon : "BD Adultes"
	Then  la vue affiche les livres du rayon : "BD Adultes"