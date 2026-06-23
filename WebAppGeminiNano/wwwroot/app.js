let session;

const chat = document.getElementById("chat");
const status = document.getElementById("status");
const input = document.getElementById("prompt");
const button = document.getElementById("send");

async function initialize() {
  if (!window.LanguageModel) {
    status.innerHTML = "Este navegador no soporta Prompt API";
    return;
  }

  const availability = await LanguageModel.availability({
    expectedInputs: [
      {
        type: "text",
        languages: ["es"],
      },
    ],
    expectedOutputs: [
      {
        type: "text",
        languages: ["es"],
      },
    ],
  });

  status.innerHTML = "Estado: " + availability;

  if (availability === "unavailable") {
    return;
  }

  session = await LanguageModel.create({
    initialPrompts: [
      {
        role: "system",
        content: "Eres un asistente técnico experto en programación.",
      },
    ],

    monitor(monitor) {
      monitor.addEventListener("downloadprogress", (e) => {
        status.innerHTML =
          "Descargando modelo: " + Math.round(e.loaded * 100) + "%";
      });
    },
  });

  status.innerHTML = "Modelo listo.";
}

function addMessage(css, text) {
  const div = document.createElement("div");

  div.className = css;

  div.textContent = text;

  chat.appendChild(div);

  chat.scrollTop = chat.scrollHeight;
}

button.onclick = async () => {
  const question = input.value.trim();

  if (!question) return;

  addMessage("user", "Tú: " + question);

  input.value = "";

  try {
    const answer = await session.prompt(question);

    addMessage("assistant", "IA: " + answer);
  } catch (e) {
    addMessage("assistant", e.message);
  }
};

initialize();
