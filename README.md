# Dictionary Parser
## [Cambridge Dictionary](https://dictionary.cambridge.org/) parser

The Dictionary Parser is an **ASP.NET Core 7.0** project designed to parse foreign dictionaries.

Now it includes an endpoint specifically created for parsing word pages from the **Cambridge Dictionary**. 

#### The following is the list of dictionary parsers being developed:

- English dictionary parser (Cambridge Dictionary, ...) - :white_check_mark: Done
- Korean dictionary parser(...) - :construction: In process
- Chinese dictionary parser(...) - :construction: In process
- ...

## License
This project is licensed under the MIT License.

## Prerequisites

To run this project on your local machine, you need to have the following dependencies installed:

- Docker: [Install Docker](https://docs.docker.com/get-docker/)

## Getting Started

Follow these steps to get the Dictionary Parser project up and running on your local machine using Docker:

1. Clone this repository to your local machine:

   ```shell
   git clone https://github.com/Kamil-Zuki/dictionary-parser.git
   ```
2. Navigate to the project directory:
  ```shell
  cd dictionary-parser
  ```
4. Build the Docker image using the provided Dockerfile:
   ```shell
   docker build -t dictionary-parser .
   ```
6. Run a Docker container using the built image:
   ```shell
   docker run -d -p 8080:80 dictionary-parser
   ```
  This command will start a Docker container running the Dictionary Parser project and expose it on http://localhost:8080.
8. Open your web browser and navigate to '**http://localhost:8080**' to access the Dictionary Parser.

## Usage
Once the Docker container is running, you can use the Dictionary Parser by following these steps:

1. Open your web browser and go to http://localhost:8080 (or the appropriate address if you changed the port mapping in the docker run command).

2. Explore the Swagger API documentation to learn more about available endpoints and their usage. You can access it at:
   ```shell
   http://localhost:8080/dictionary-parser/swagger
   ```
The Swagger UI provides an interactive interface to explore and test the API.

## Contributing
Contributions to the Dictionary Parser project are welcome! If you encounter any issues or have suggestions for improvements, please create a new issue or submit a pull request.


