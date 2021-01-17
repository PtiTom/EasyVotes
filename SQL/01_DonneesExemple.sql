-- Création d'un jeu de données d'exemple / Test
DECLARE @IdSession int, @IdPremiereQuestion int

INSERT INTO Vote.SessionVote SELECT 'Session Exemple', GETDATE(), NULL, 'Thomas', NULL
SELECT @IdSession = @@IDENTITY

INSERT INTO Vote.Inscrit SELECT 'Thomas', @IdSession


INSERT INTO Vote.Vote SELECT @IdSession, 'Première question : Etes-vous d''accord ?', 1, 0
SELECT @IdPremiereQuestion = @@IDENTITY

INSERT INTO Vote.Choix SELECT @IdPremiereQuestion, 1, 'Oui'
INSERT INTO Vote.Choix SELECT @IdPremiereQuestion, 2, 'Non'
INSERT INTO Vote.Choix SELECT @IdPremiereQuestion, 3, 'Ni pour, ni contre, bien au contraire'
INSERT INTO Vote.Choix SELECT @IdPremiereQuestion, 4, 'La réponse D.'
