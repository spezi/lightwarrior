
// Function to make an element draggable
function makeDraggable(element) {

    const container = document.getElementById('mapping');
    const point_event = new Event("phx:pointchange");

    let isDragging = false;
    let offsetX = 0;
    let offsetY = 0;

    element.addEventListener('mousedown', (event) => {
        isDragging = true;
        offsetX = event.clientX - element.getBoundingClientRect().left;
        offsetY = event.clientY - element.getBoundingClientRect().top;
    });

    document.addEventListener('mousemove', (event) => {
        if (isDragging) {
            const containerRect = container.getBoundingClientRect();
            let newX = event.clientX - containerRect.left - offsetX;
            let newY = event.clientY - containerRect.top - offsetY;
            //console.log(newX)
            // Constrain the draggable element within the container
            newX = Math.max(0, Math.min(container.clientWidth - element.clientWidth, newX));
            newY = Math.max(0, Math.min(container.clientHeight - element.clientHeight, newY));

            element.style.left = newX + 'px';
            element.style.top = newY + 'px';

            //console.log(element.style.left)
            //console.log(element.style.top)
        }
    });

    document.addEventListener('mouseup', (event) => {
        //console.log(event.target.style.left)
        window.dispatchEvent(point_event, {id: event.target.id, left: event.target.style.left, top: event.target.style.top});
        isDragging = false;
    });
}



export default makeDraggable;