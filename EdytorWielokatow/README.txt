Klawiszologia:
 - Przesuwanie wierzchołka/krawędzi/całego wielokąta:
	Tylko w stanie aplikacji LookingAtPolygon. Lewy wciśnięty przycisk myszy. Aby przesunąć cały wielokąt trzeba kliknąć w jego bounding box (ale nie wierzchołek lub krawędź).
 - Tworzenie wielkąta:
	Tylko w stanie aplikacji CreatingPolygon.Wystarczy kliknąć lewym przyciskiem myszy w miejsce gdzie ma być nowy wierzchołek. Aby zakończyć tworzenie muszą być przynajmniej 3 wierzchołki i wtedy trzeba kliknąć na pierwszy wierzchołek.
 - Prawy przycisk myszy:
	Otwiera menu w stanie aplikacji LookingAtPolygon. Jest menu wierzchołkowe, krawędzi oraz ogólne w którym można usunąć cały wielokąt

Opis struktury danych:
	Cały wieloką jest trzymany jako lista dwukierunkowa krawędzi, gdzie każda krawędź ma referencje do odpowiednich wierzchołków. Cała ta struktura jest obsługiwana przez klasę EdgesList. Istnieją w niej dwa kierunki Next i Prev, które są ustalane przy tworzeniu wielokąta. Każda krawędź ma funkcję ChangeVertexPos(Vertex changed, Vertex changing), która zmienia pozycję wierzchołka krawędzi changing, względem wierzchołka krawędzi changed. 
	
Opis algorytmu "relacji":
	Jest wywoływana funkcja ChangeVertexPos na zmiane na następnej krawędzi (Next) i poprzedniej krawędzi (Prev), od przesuwanego wierzchołka lub dwóch w przypadku przesuwania krawędzi. Dodawane są kolejne krawędzie na zmiane z każdej strony, dopóki nie będzie zwykłej krawędzi (nie sąsiadującej z bezierem), lub następny wierzchołek nie będzie zablokowany (IsLocked). Na końcu ostatnia krawędź jest sprawdzana czy jest poprawna, po wyliczeniu jej wierzchołków z obu stron. Jeśli nie jest wszystkie zmiany są cofane.

