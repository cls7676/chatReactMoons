import { IAsk, IAskInput } from '../model/Ask';
import { IKeyConfig } from '../model/KeyConfig';
import { SemanticKernel } from './SemanticKernel';

export interface IPlanCreated {
    onPlanCreated: (ask: IAsk, plan: string) => void;
}

export class TaskRunner {
    // eslint-disable-next-line @typescript-eslint/space-before-function-paren
    constructor(
        private readonly sk: SemanticKernel,
        private readonly keyConfig: IKeyConfig,
        private readonly maxSteps: number = 10,
    ) {}

    runTask = async (
        taskDescription: string,
        taskResponseFormat?: string,
        skills?: string[],
        onPlanCreated?: (ask: IAsk, plan: string) => void,
        onTaskCompleted?: (ask: IAsk, result: string) => void,
    ) => {
        var createPlanRequest = taskDescription;

        if (taskResponseFormat !== undefined) {
            createPlanRequest = `${createPlanRequest} MUST RETURN A SINGLE RESULT IN THIS FORMAT: ${taskRe