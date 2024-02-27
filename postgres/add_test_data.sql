INSERT INTO Projects (name, code) VALUES
('Жилой комплекс "Зеленый оазис"', 'GCZ001'),
('Торгово-офисный центр "Экоплаза"', 'TOE202'),
('Строительство энергоэффективных домов', 'SED303'),
('Реконструкция парковочного комплекса', 'RPC404');

INSERT INTO Materials (project_id, name, class, weight, length) VALUES
(1, 'Фундаментные блоки', 'foundation', 2500, 2.5),
(2, 'Стальные балки','support', 1500, 3.7),
(1, 'Экологические ограждения', 'fencing', 800, 1.9),
(3, 'Утеплитель для стен', 'insulation', 300, 4.2);

INSERT INTO Classes (name) VALUES
('foundation'),
('support'),
('fencing'),
('insulation');

INSERT INTO Tests (name) VALUES
('Нагрузка на фундамент'),
('Прочность строительных материалов'),
('Теплоизоляция помещений');