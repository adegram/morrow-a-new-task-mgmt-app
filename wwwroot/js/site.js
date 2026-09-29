const taskDialog = document.getElementById('task-dialog');
const taskForm = document.getElementById('task-form');
const pageView = taskForm?.querySelector('[name="view"]')?.value || 'today';

function openTaskDialog(mode) {
    if (!taskDialog || !taskForm) return;
    taskForm.action = mode === 'edit' ? `/?handler=Update&view=${encodeURIComponent(pageView)}` : `/?handler=Create&view=${encodeURIComponent(pageView)}`;
    taskForm.reset();
    taskForm.querySelector('[name="view"]').value = pageView;
    document.getElementById('dialog-title').textContent = mode === 'edit' ? 'Edit task' : 'Create a task';
    document.getElementById('dialog-submit').innerHTML = mode === 'edit' ? 'Save changes <span>↗</span>' : 'Add task <span>↗</span>';
    document.getElementById('dialog-delete').hidden = mode !== 'edit';
    if (mode === 'edit') {
        const id = document.getElementById('edit-id');
        if (id) id.remove();
    }
    taskDialog.showModal();
    window.setTimeout(() => document.getElementById('field-title').focus(), 50);
}

function deleteCurrentTask() {
    if (!taskForm || !window.confirm('Delete this task? This action cannot be undone.')) return;
    taskForm.action = `/?handler=Delete&view=${encodeURIComponent(pageView)}`;
    taskForm.requestSubmit();
}

function openEdit(button) {
    openTaskDialog('edit');
    const hidden = document.createElement('input');
    hidden.type = 'hidden'; hidden.name = 'id'; hidden.id = 'edit-id';
    taskForm.appendChild(hidden);
    hidden.value = button.dataset.id;
    document.getElementById('field-title').value = button.dataset.title || '';
    document.getElementById('field-notes').value = button.dataset.notes || '';
    document.getElementById('field-category').value = button.dataset.category || 'Personal';
    document.getElementById('field-priority').value = button.dataset.priority || 'Medium';
    document.getElementById('field-date').value = button.dataset.date || '';
}

function closeTaskDialog() { if (taskDialog?.open) taskDialog.close(); }
taskDialog?.addEventListener('click', event => { if (event.target === taskDialog) closeTaskDialog(); });

function filterTasks(query) {
    const value = query.trim().toLocaleLowerCase();
    document.querySelectorAll('.task-row').forEach(row => { row.hidden = !row.dataset.title.includes(value); });
}

function sortTasks(mode) {
    const list = document.getElementById('task-list');
    if (!list) return;
    [...list.children].sort((a, b) => {
        if (mode === 'priority') {
            const rank = { high: 0, medium: 1, low: 2 };
            return rank[a.dataset.priority] - rank[b.dataset.priority];
        }
        if (mode === 'alphabetical') return a.dataset.title.localeCompare(b.dataset.title);
        return a.dataset.date.localeCompare(b.dataset.date);
    }).forEach(row => list.appendChild(row));
}
