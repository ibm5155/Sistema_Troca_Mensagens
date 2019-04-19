# Sistema_Troca_Mensagens

The project aims to build a secure, efficient, reliable and highly available service for sending textual messages between two distributed processes. To this end, the service is composed of an active replication-based processing server and a data replication-based persistence server, according to the situation illustrated in Figure 1, in which the process corresponding to the user Bob sends messages to the process corresponding to the user Alice. The processing server performs encryption / decrypt operations on the exchanged messages, using the RSA Algorithm. The persistence server stores encrypted messages. An algorithm of choice is applied to the processing server implementation, while a distributed mutual exclusion algorithm is applied to the persistence server implementation. The project is composed of four phases of implementation, as described in the subsequent sections, developed according to the schedule shown in the table below.

===============================================================================

O projeto visa a construção de um serviço seguro, eﬁciente, conﬁável e de alta disponibilidade para envio de mensagens textuais entre dois processos distribuídos. Para tanto, o serviço é composto por um servidor de processamento baseado em replicação ativa e um servidor de persistência baseado em replicação de dados, conforme a situação ilustrada pela Figura 1, na qual o processo correspondente ao usuário Bob envia mensagens para o processo correspondente à usuária Alice. O servidor de processamento realiza operações de criptograﬁa (encrypt/decrypt) nas mensagens trocadas, usando o Algoritmo de RSA. O servidor de persistência faz o armazenamento das mensagens criptografadas. É aplicado um algoritmo de eleição na implementação do servidor de processamento, enquanto é aplicado um algoritmo de exclusão mútua distribuída na implementação do servidor de persistência. O projeto é composto por quatro fases de implementação, conforme descrito nas seções subsequentes, desenvolvidas de acordo com o cronograma mostrado na tabela abaixo.
